using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShootBalls.Gameplay.Attacking;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Pawn;
using ShootBalls.Gameplay.Weapons;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class Projectile : IPawn,
		IFixedTickable,
		IPoolable<Projectile.Settings, IMemoryPool>,
		IDisposable
	{
		public event System.Action<Projectile> Disposed;

		public Rigidbody2D Body => _body;
		public Expiry Lifetimer { get; }

		private readonly Rigidbody2D _body;
		private readonly CharacterMotor _motor;
		private readonly Collider2D _collider;
		private readonly AttackController _attackController;
		private readonly ProjectileCollisionHandler[] _collisionHandlers;
		private readonly IDamageData _collisionData;

		private Settings _settings;
		private IMemoryPool _pool;
		private CancellationTokenSource _cancelSource;
		private Ball _ball;

		public Projectile( Rigidbody2D body,
			CharacterMotor motor,
			Collider2D collider,
			AttackController attackController,
			ProjectileCollisionHandler[] collisionHandlers,
			OnCollisionEnter2DBroadcaster collisionEnter,
			OnTriggerEnter2DBroadcaster ballEnterDetector,
			OnTriggerExit2DBroadcaster ballExitDetector )
		{
			_body = body;
			_motor = motor;
			_collider = collider;
			_attackController = attackController;
			_collisionHandlers = collisionHandlers;

			Lifetimer = new Expiry( 0, this );
			_collisionData = new UnhandledDamageData() { Causer = this };

			collisionEnter.Entered += OnCollisionEnter;
			ballEnterDetector.Entered += OnBallFound;
			ballExitDetector.Exited += OnBallLost;
		}

		private void OnCollisionEnter( Collision2D collision )
		{
			_attackController.DealDamage( new AttackController.Request()
			{
				Collision = collision,
				Instigator = _settings.Owner,
				Causer = this,
				Settings = _settings.AttackSettings
			} );

			var contact = collision.GetContact( 0 );
			_collisionData.Instigator = _settings.Owner;
			_collisionData.HitPosition = contact.point;
			_collisionData.HitNormal = contact.normal;
			foreach ( var myCollision in _collisionHandlers )
			{
				myCollision.Handle( this, _collisionData );
			}
		}

		private void OnBallFound( Collider2D collision )
		{
			if ( collision.TryResolveFromBodyContext<Ball>( out var ball ) )
			{
				_ball = ball;
			}
		}

		private void OnBallLost( Collider2D collision )
		{
			if ( collision.TryResolveFromBodyContext<Ball>( out _ ) )
			{
				_ball = null;
			}
		}

		public void OnSpawned( Settings settings, IMemoryPool pool )
		{
			_settings = settings;
			_pool = pool;

			_body.gameObject.SetActive( true );

			Lifetimer.SetLifetime( settings.Lifetime );

			if ( _cancelSource == null )
			{
				_cancelSource = CancellationTokenSource.CreateLinkedTokenSource( _body.GetCancellationTokenOnDestroy() );
			}
		}

		public void Launch( Vector2 velocity )
		{
			_body.AddForce( velocity, ForceMode2D.Impulse );

			_motor.ToggleRotationUpdate( false );
			_motor.SetDesiredVelocity( velocity.normalized );

			Lifetimer.Tick( _cancelSource.Token ).Forget();
		}

		public void FixedTick()
		{
			Vector2 moveDirection = _body.transform.up;

			if ( _ball != null )
			{
				moveDirection = (_ball.Body.position - _body.position).normalized;

				_motor.ToggleRotationUpdate( true );
				_motor.SetDesiredRotation( moveDirection );
			}

			if ( _collider.enabled )
			{
				_motor.SetDesiredVelocity( moveDirection );
				_motor.FixedTick();
			}
		}

		public void Dispose()
		{
			_pool?.Despawn( this );

			foreach ( var dmgHandler in _collisionHandlers )
			{
				dmgHandler.Dispose();
			}
		}

		public void OnDespawned()
		{
			_pool = null;
			Disposed?.Invoke( this );

			_ball = null;

			_body.gameObject.SetActive( false );
		}

		public void SetCollisionActive( bool isActive )
		{
			_collider.enabled = isActive;
		}

		public class Factory : PlaceholderFactory<Settings, Projectile> { }

		[System.Serializable]
		public class Settings
		{
			public IPawn Owner { get; set; }

			public float Lifetime;

			[FoldoutGroup( "Attack" ), HideLabel]
			public AttackController.Settings AttackSettings;
		}
	}
}

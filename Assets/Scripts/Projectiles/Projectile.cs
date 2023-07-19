using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Weapons;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class Projectile : IOrientation,
		IFixedTickable,
		IPoolable<Projectile.Settings, IMemoryPool>,
		IDisposable
	{
		public event System.Action<Projectile> Disposed;

		public Vector2 Position {
			get => _body.position;
			set => _body.position = value;
		}
		public Quaternion Rotation {
			get => _body.rotation.To2DRotation();
			set => _body.SetRotation( value );
		}
		public Transform Parent {
			get => _settings.Owner;
			set => _settings.Owner = value;
		}
		public Expiry Lifetimer { get; }

		private readonly Rigidbody2D _body;
		private readonly CharacterMotor _motor;
		private readonly Collider2D _collider;
		private readonly IProjectileDamageHandler[] _damageHandlers;
		private readonly SignalBus _signalBus;
		private Settings _settings;
		private IMemoryPool _pool;
		private CancellationTokenSource _cancelSource;
		private Ball _ball;

		public Projectile( Rigidbody2D body,
			CharacterMotor _motor,
			Collider2D collider,
			IProjectileDamageHandler[] damageHandlers,
			OnCollisionEnter2DBroadcaster collisionEnter,
			OnTriggerEnter2DBroadcaster ballEnterDetector,
			OnTriggerExit2DBroadcaster ballExitDetector,
			SignalBus signalBus )
		{
			_body = body;
			this._motor = _motor;
			_collider = collider;
			_damageHandlers = damageHandlers;
			_signalBus = signalBus;

			Lifetimer = new Expiry( 0, this );

			collisionEnter.Entered += OnCollisionEnter;
			ballEnterDetector.Entered += OnBallFound;
			ballExitDetector.Exited += OnBallLost;
		}

		private void OnCollisionEnter( Collision2D collision )
		{
			var contact = collision.GetContact( 0 );
			var dmgSignal = new DamageDeliveredSignal() 
			{ 
				HitBody = collision.rigidbody,
				HitDirection = contact.normal
			};

			foreach ( var dmgHandler in _damageHandlers )
			{
				dmgHandler.Handle( this, dmgSignal );
			}

			_signalBus.FireId( "Damaged", new FxSignal()
			{
				Position = contact.point,
				Direction = contact.normal,
				Parent = _body.transform
			} );
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

			_motor.SetDesiredVelocity( moveDirection );
			_motor.FixedTick();
		}

		public void Dispose()
		{
			_pool?.Despawn( this );

			foreach ( var dmgHandler in _damageHandlers )
			{
				dmgHandler.Dispose();
			}
		}

		public void OnDespawned()
		{
			_pool = null;
			Disposed?.Invoke( this );

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
			public Transform Owner { get; set; }

			public float Lifetime;
		}
	}
}

using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class Ball : IPawn,
		IFixedTickable
	{
		public Rigidbody2D Body => _body;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly CharacterMotor _motor;
		private readonly SignalBus _signalBus;
		private readonly GameModel _gameModel;

		private int _health;
		private float _healDelayEndTime;
		private float _healTimer;
		private float _stunEndTime;

		public Ball( Settings settings,
			Rigidbody2D body,
			CharacterMotor motor,
			OnCollisionEnter2DBroadcaster collisionEnter,
			SignalBus signalBus,
			GameModel gameModel )
		{
			_settings = settings;
			_body = body;
			_motor = motor;
			_signalBus = signalBus;
			_gameModel = gameModel;
			collisionEnter.Entered += OnCollisionEnter;

			_health = settings.Health;
		}

		private void OnCollisionEnter( Collision2D collision )
		{
			if ( collision.collider.TryResolveFromBodyContext<Projectile>( out _ ) )
			{
				TakeDamage( collision );
			}
		}

		private void TakeDamage( Collision2D collision )
		{
			if ( _health > 0 )
			{
				--_health;
				_healTimer = 0;
				_healDelayEndTime = Time.timeSinceLevelLoad + _settings.HealDelay;

				if ( _health <= 0 )
				{
					_stunEndTime = Time.timeSinceLevelLoad + _settings.StunDuration;
				}
			}

			var contact = collision.GetContact( 0 );
			_signalBus.FireId( "Damaged", new FxSignal()
			{
				Position = contact.point,
				Direction = contact.normal,
				Parent = _body.transform
			} );
		}

		public void FixedTick()
		{
			if ( HandleStunState() )
			{
				return;
			}

			TryHeal();
			FollowPlayer();
		}

		private bool HandleStunState()
		{
			if ( _stunEndTime <= Time.timeSinceLevelLoad )
			{
				if ( _health <= 0 )
				{
					_health = _settings.Health;
				}
				return false;
			}

			_motor.SetDesiredVelocity( Vector2.zero );
			_motor.FixedTick();

			return true;
		}

		private void TryHeal()
		{
			if ( _health >= _settings.Health || _healDelayEndTime > Time.timeSinceLevelLoad )
			{
				return;
			}

			_healTimer += Time.deltaTime;
			if ( _healTimer >= _settings.HealRatePerHP )
			{
				++_health;
				_healTimer = 0;
			}
		}

		private void FollowPlayer()
		{
			if ( _gameModel.Player != null )
			{
				Vector2 moveDir = (_gameModel.Player.Body.position - _body.position).normalized;

				_motor.SetDesiredVelocity( moveDir );
				_motor.FixedTick();
			}
		}

		public class Factory : PlaceholderFactory<Ball> { }

		[System.Serializable]
		public class Settings
		{
			[BoxGroup( "Health" ), MinValue( 1 )]
			public int Health = 3;
			[BoxGroup( "Health" ), MinValue( 0 )]
			public float HealDelay = 0.5f;
			[BoxGroup( "Health" ), MinValue( 0 )]
			public float HealRatePerHP = 1f / 3f;

			[BoxGroup( "Recovery" ), MinValue( 0 )]
			public float StunDuration = 3;
			[BoxGroup( "Recovery" ), MinValue( 0 )]
			public float InvulnerableDuration = 5;
		}
	}
}

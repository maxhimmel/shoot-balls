using System.Collections.Generic;
using System.Linq;
using ShootBalls.Gameplay.Attacking;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Pawn;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class Ball : IPawn,
		IDamageable,
		IStunnable,
		IFixedTickable
	{
		public Rigidbody2D Body => _body;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly CharacterMotor _motor;
		private readonly AttackController _attackController;
		private readonly Dictionary<System.Type, IDamageHandler> _damageHandlers;
		private readonly SignalBus _signalBus;
		private readonly GameModel _gameModel;

		private float _health;
		private float _healDelayEndTime;
		private float _healTimer;
		private float _stunEndTime;
		private bool _isStunLaunched;
		private IDamageData _recentDamage;

		public Ball( Settings settings,
			Rigidbody2D body,
			CharacterMotor motor,
			AttackController attackController,
			IDamageHandler[] damageHandlers,
			SignalBus signalBus,
			GameModel gameModel,
			OnCollisionEnter2DBroadcaster collisionEnter )
		{
			_settings = settings;
			_body = body;
			_motor = motor;
			_attackController = attackController;
			_damageHandlers = damageHandlers.ToDictionary( handler => handler.GetType() );
			_signalBus = signalBus;
			_gameModel = gameModel;

			_health = settings.Health;

			collisionEnter.Entered += OnCollisionEnter;
		}

		public bool TakeDamage( IDamageData data )
		{
			bool wasDamaged = false;
			if ( _damageHandlers.TryGetValue( data.HandlerType, out var handler ) )
			{
				_recentDamage = data;

				if ( handler.Handle( this, data ) )
				{
					wasDamaged = true;
				}
			}

			_signalBus.FireId( wasDamaged ? "Damaged" : "Deflected", new FxSignal()
			{
				Position = data.HitPosition,
				Direction = data.HitNormal,
				Parent = _body.transform
			} );

			return wasDamaged;
		}

		void IStunnable.OnStunHit( float damage )
		{
			if ( _health > 0 )
			{
				_health -= damage;
				_healTimer = 0;
				_healDelayEndTime = Time.timeSinceLevelLoad + _settings.HealDelay;

				if ( _health <= 0 )
				{
					OnStunned();
				}
			}
		}

		private void OnStunned()
		{
			_stunEndTime = Time.timeSinceLevelLoad + _settings.StunDuration;

			_signalBus.FireId( "Stunned", new FxSignal()
			{
				Position = _body.position,
				Direction = _body.transform.up,
				Parent = _body.transform
			} );
		}

		void IStunnable.OnDirectHit( float damage )
		{
			_isStunLaunched = true;

			_signalBus.FireId( "Launched", new FxSignal()
			{
				Position = _recentDamage.HitPosition,
				Direction = -_recentDamage.HitNormal,
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
			if ( !IsStunned() )
			{
				if ( _health <= 0 )
				{
					OnRecovered();
				}
				return false;
			}

			_motor.SetDesiredVelocity( Vector2.zero );
			_motor.FixedTick();

			return true;
		}

		public bool IsStunned()
		{
			return _stunEndTime > Time.timeSinceLevelLoad;
		}

		public void OnRecovered()
		{
			_health = _settings.Health;
			_isStunLaunched = false;

			_signalBus.FireId( "Recovered", new FxSignal()
			{
				Position = _body.position,
				Direction = _body.transform.up,
				Parent = _body.transform
			} );
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

		private void OnCollisionEnter( Collision2D collision )
		{
			if ( _isStunLaunched )
			{
				_attackController.DealDamage( new AttackController.Request()
				{
					Collision = collision,
					Instigator = this,
					Causer = this,
					Settings = _settings.LaunchAttack
				} );
			}
		}

		public class Factory : PlaceholderFactory<Ball> { }

		[System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "Health" ), MinValue( 1 )]
			public float Health = 3;
			[FoldoutGroup( "Health" ), MinValue( 0 )]
			public float HealDelay = 0.5f;
			[FoldoutGroup( "Health" ), MinValue( 0 )]
			public float HealRatePerHP = 1f / 3f;

			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float StunDuration = 3;
			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float InvulnerableDuration = 5;

			[FoldoutGroup( "Launch Attack" ), HideLabel]
			public AttackController.Settings LaunchAttack;
		}
	}
}

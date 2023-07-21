using System.Collections.Generic;
using System.Linq;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Pawn;
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
		private readonly Dictionary<System.Type, IDamageHandler> _damageHandlers;
		private readonly SignalBus _signalBus;
		private readonly GameModel _gameModel;

		private int _health;
		private float _healDelayEndTime;
		private float _healTimer;
		private float _stunEndTime;

		public Ball( Settings settings,
			Rigidbody2D body,
			CharacterMotor motor,
			IDamageHandler[] damageHandlers,
			SignalBus signalBus,
			GameModel gameModel )
		{
			_settings = settings;
			_body = body;
			_motor = motor;
			_damageHandlers = damageHandlers.ToDictionary( handler => handler.GetType() );
			_signalBus = signalBus;
			_gameModel = gameModel;

			_health = settings.Health;
		}

		public bool TakeDamage( IDamageData data )
		{
			bool wasDamaged = false;
			if ( _damageHandlers.TryGetValue( data.HandlerType, out var handler ) )
			{
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

		public bool Hit()
		{
			if ( _health > 0 )
			{
				--_health;
				_healTimer = 0;
				_healDelayEndTime = Time.timeSinceLevelLoad + _settings.HealDelay;

				if ( _health <= 0 )
				{
					OnStunned();
					return true;
				}
			}

			return false;
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

		public class Factory : PlaceholderFactory<Ball> { }

		[System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "Health" ), MinValue( 1 )]
			public int Health = 3;
			[FoldoutGroup( "Health" ), MinValue( 0 )]
			public float HealDelay = 0.5f;
			[FoldoutGroup( "Health" ), MinValue( 0 )]
			public float HealRatePerHP = 1f / 3f;

			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float StunDuration = 3;
			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float InvulnerableDuration = 5;
		}
	}
}

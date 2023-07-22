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
		private readonly StunController _stunController;
		private readonly AttackController _attackController;
		private readonly Dictionary<System.Type, IDamageHandler> _damageHandlers;
		private readonly SignalBus _signalBus;
		private readonly GameModel _gameModel;

		private float _healDelayEndTime;
		private float _healTimer;
		private bool _isStunLaunched;
		private IDamageData _recentDamage;

		public Ball( Settings settings,
			Rigidbody2D body,
			CharacterMotor motor,
			StunController stunController,
			AttackController attackController,
			IDamageHandler[] damageHandlers,
			SignalBus signalBus,
			GameModel gameModel,
			OnCollisionEnter2DBroadcaster collisionEnter )
		{
			_settings = settings;
			_body = body;
			_motor = motor;
			_stunController = stunController;
			_attackController = attackController;
			_damageHandlers = damageHandlers.ToDictionary( handler => handler.GetType() );
			_signalBus = signalBus;
			_gameModel = gameModel;

			collisionEnter.Entered += OnCollisionEnter;

			stunController.Recovered += OnRecovered;
		}

		public bool TakeDamage( IDamageData data )
		{
			bool wasDamaged = false;

			if ( _damageHandlers.TryGetValue( data.HandlerType, out var handler ) )
			{
				_recentDamage = data;
				wasDamaged = handler.Handle( this, data );
			}

			_signalBus.FireId( wasDamaged ? "Damaged" : "Deflected", new FxSignal()
			{
				Position = data.HitPosition,
				Direction = -data.HitNormal,
				Parent = _body.transform
			} );

			return wasDamaged;
		}

		void IStunnable.OnStunHit( float damage )
		{
			if ( !_stunController.Hit( damage ) )
			{
				_healTimer = 0;
				_healDelayEndTime = Time.timeSinceLevelLoad + _settings.HealDelay;
			}
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
			if ( !_stunController.Tick() )
			{
				return false;
			}

			_motor.SetDesiredVelocity( Vector2.zero );
			_motor.FixedTick();

			return true;
		}

		public bool IsStunned()
		{
			return _stunController.IsStunned;
		}

		private void OnRecovered()
		{
			_isStunLaunched = false;
		}

		private void TryHeal()
		{
			if ( !_stunController.IsDamaged || _healDelayEndTime > Time.timeSinceLevelLoad )
			{
				return;
			}

			_healTimer += Time.deltaTime;
			if ( _healTimer >= _settings.HealRatePerHP )
			{
				_stunController.AddStunPoints( 1 );
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
			[FoldoutGroup( "Health" ), HideLabel]
			public StunController.Settings Stun;

			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float HealDelay = 0.5f;
			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float HealRatePerHP = 1f / 3f;
			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float InvulnerableDuration = 5;

			[FoldoutGroup( "Launch Attack" ), HideLabel]
			public AttackController.Settings LaunchAttack;
		}
	}
}

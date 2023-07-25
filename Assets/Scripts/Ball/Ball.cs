using System.Collections.Generic;
using System.Linq;
using ShootBalls.Gameplay.Attacking;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.LevelPieces;
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
		private readonly DamageHandlerController _damageController;
		private readonly SignalBus _signalBus;
		private readonly GameModel _gameModel;

		private float _healDelayEndTime;
		private float _healTimer;
		private bool _isStunLaunched;

		public Ball( Settings settings,
			Rigidbody2D body,
			CharacterMotor motor,
			StunController stunController,
			AttackController attackController,
			DamageHandlerController damageController,
			SignalBus signalBus,
			GameModel gameModel,
			OnCollisionEnter2DBroadcaster collisionEnter )
		{
			_settings = settings;
			_body = body;
			_motor = motor;
			_stunController = stunController;
			_attackController = attackController;
			_damageController = damageController;
			_signalBus = signalBus;
			_gameModel = gameModel;

			collisionEnter.Entered += OnCollisionEnter;
			stunController.Recovered += OnRecovered;
		}

		public bool TakeDamage( IDamageData data )
		{
			InfluenceHitNormal( ref data );

			return _damageController.TakeDamage( this, data );
		}

		private void InfluenceHitNormal( ref IDamageData data )
		{
			if ( _settings.HomingInfluence <= 0 )
			{
				return;
			}
			if ( !IsStunned() )
			{
				return;
			}

			Brick bestBrick = null;
			float bestAlignment = Mathf.Infinity;
			foreach ( var brick in _gameModel.ActiveBricks )
			{
				Vector2 selfToBrick = brick.Body.position - _body.position;
				float distance = selfToBrick.magnitude;

				float dot = Vector2.Dot( selfToBrick / distance, -data.HitNormal );
				float angle = Mathf.Acos( dot ) * 180f / Mathf.PI;

				if ( angle <= _settings.MinAlignmentAngle )
				{
					if ( angle < bestAlignment )
					{
						bestAlignment = angle;
						bestBrick = brick;
					}
				}
			}

			if ( bestBrick != null )
			{
				Vector2 influenceVector = (_body.position - bestBrick.Body.position).normalized;

				_signalBus.TryFireId( "Launched_HomingInfluence", new FxSignal()
				{
					Position = _body.position,
					Direction = -influenceVector,
					Parent = _body.transform
				} );
				_signalBus.TryFireId( "Launched_RawHitDirection", new FxSignal()
				{
					Position = _body.position,
					Direction = -data.HitNormal,
					Parent = _body.transform
				} );

				data.HitNormal = Vector3.Slerp( data.HitNormal, influenceVector, _settings.HomingInfluence );
			}
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
				Position = _body.position,
				Direction = _body.velocity.normalized,
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
			_attackController.DealDamage( new AttackController.Request()
			{
				Collision = collision,
				Instigator = this,
				Causer = this,
				Settings = _isStunLaunched 
					? _settings.LaunchAttack 
					: _settings.SeekAttack
			} );
		}

		public class Factory : PlaceholderFactory<Ball> { }

		[System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "Health" ), HideLabel]
			public StunController.Settings Stun;
			[FoldoutGroup( "Health" ), HideLabel]
			public DamageHandlerController.Settings Damage;

			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float HealDelay = 0.5f;
			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float HealRatePerHP = 1f / 3f;
			[FoldoutGroup( "Recovery" ), MinValue( 0 )]
			public float InvulnerableDuration = 5;

			[FoldoutGroup( "Brick Homing" ), Range( 0, 1 )]
			public float HomingInfluence;
			[FoldoutGroup( "Brick Homing" ), Range( 0, 180 )]
			public float MinAlignmentAngle;

			[FoldoutGroup( "Attacks" )]
			public AttackController.Settings LaunchAttack;
			[FoldoutGroup( "Attacks" )]
			public AttackController.Settings SeekAttack;
		}
	}
}

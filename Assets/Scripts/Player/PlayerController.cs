using Cysharp.Threading.Tasks;
using ShootBalls.Gameplay.Cameras;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Pawn;
using ShootBalls.Gameplay.Weapons;
using ShootBalls.Installers;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Player
{
	public class PlayerController : IPawn,
		IInitializable,
		ITickable,
		IFixedTickable,
		IPushable,
		IDamageable,
		IStunnable
	{
		public event System.Action Died;

		public Rigidbody2D Body => _body;
		private bool IsDead => _health <= 0;

		private readonly Settings _settings;
		private readonly Rewired.Player _input;
		private readonly CharacterMotor _motor;
		private readonly IRotationMotor _rotation;
		private readonly DodgeController _dodgeController;
		private readonly DamageHandlerController _damageController;
		private readonly StunController _stunController;
		private readonly Gun.Factory _gunFactory;
		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;

		private float _health;
		private Gun _primaryGun, _secondaryGun;

		public PlayerController( Settings settings,
			Rewired.Player input,
			CharacterMotor motor,
			IRotationMotor rotation,
			DodgeController dodgeController,
			DamageHandlerController damageController,
			StunController stunController,
			Gun.Factory gunFactory,
			Rigidbody2D body,
			SignalBus signalBus )
		{
			_settings = settings;
			_input = input;
			_motor = motor;
			_rotation = rotation;
			_dodgeController = dodgeController;
			_damageController = damageController;
			_stunController = stunController;
			_gunFactory = gunFactory;
			_body = body;
			_signalBus = signalBus;
		}

		public void Initialize()
		{
			_primaryGun = _gunFactory.Create( _settings.PrimaryWeaponPrefab );
			_secondaryGun = _gunFactory.Create( _settings.SecondaryWeaponPrefab );

			_primaryGun.SetOwner( this );
			_secondaryGun.SetOwner( this );

			_health = _settings.Health;
			_stunController.Stunned += OnStunned;
		}

		public void Tick()
		{
			if ( IsDead || _stunController.Tick() )
			{
				return;
			}

			HandleMovement();

			if ( _dodgeController.IsDodging )
			{
				return;
			}

			HandleRotating();

			HandleGunFiring();
		}

		private void HandleMovement()
		{
			var moveInput = _input.GetClampedAxis2D( ReConsts.Action.Horizontal, ReConsts.Action.Vertical );

			if ( _input.GetButtonDown( ReConsts.Action.Dodge ) )
			{
				Vector3 dodgeDirection = moveInput != Vector2.zero
					? moveInput.normalized
					: _body.transform.up;

				if ( _dodgeController.Dodge( dodgeDirection ) )
				{
					_primaryGun.StopFiring();
					_secondaryGun.StopFiring();
				}
			}

			if ( _dodgeController.IsDodging )
			{
				_motor.SetDesiredVelocity( _dodgeController.Direction );
			}
			else
			{
				_motor.SetDesiredVelocity( moveInput );
			}
		}

		private void HandleRotating()
		{
			var aimInput = _input.GetClampedAxis2D( ReConsts.Action.AimHorizontal, ReConsts.Action.AimVertical );
			if ( aimInput != Vector2.zero )
			{
				_rotation.SetDesiredRotation( aimInput );
			}
		}

		private void HandleGunFiring()
		{
			if ( _input.GetButton( ReConsts.Action.PrimaryFire ) )
			{
				_primaryGun.StartFiring();
			}
			else if ( _input.GetButtonUp( ReConsts.Action.PrimaryFire ) )
			{
				_primaryGun.StopFiring();
			}

			if ( _input.GetButton( ReConsts.Action.SecondaryFire ) )
			{
				_secondaryGun.StartFiring();
			}
			else if ( _input.GetButtonUp( ReConsts.Action.SecondaryFire ) )
			{
				_secondaryGun.StopFiring();
			}
		}

		public void FixedTick()
		{
			if ( !IsDead && !IsStunned() )
			{
				_motor.FixedTick();
				_rotation.FixedTick();
			}
		}

		void IPushable.Push( Vector2 velocity )
		{
			_body.AddForce( velocity, ForceMode2D.Impulse );
		}

		public bool TakeDamage( IDamageData data )
		{
			return _damageController.TakeDamage( this, data );
		}

		public bool IsStunned()
		{
			return _stunController.IsStunned;
		}

		void IStunnable.OnStunHit( float damage )
		{
			_stunController.Hit( damage );
		}

		private void OnStunned()
		{
			_motor.SetDesiredVelocity( Vector2.zero );

			_primaryGun.StopFiring();
			_secondaryGun.StopFiring();
		}

		void IStunnable.OnDirectHit( float damage )
		{
			if ( !IsDead )
			{
				_health -= damage;
				if ( IsDead )
				{
					OnDead();
				}
			}
		}

		private void OnDead()
		{
			_health = 0;

			_signalBus.FireId( "Dead", new FxSignal()
			{
				Position = _body.position,
				Direction = -_damageController.RecentDamage.HitNormal,
				Parent = _body.transform
			} );

			Died?.Invoke();

			DelayedCleanup().Forget();
		}

		private async UniTaskVoid DelayedCleanup()
		{
			var cancelToken = _body.GetCancellationTokenOnDestroy();
			await TaskHelpers.DelaySeconds( _settings.DeathAnimDuration, cancelToken );

			_body.gameObject.SetActive( false );
		}

		public class Factory : PlaceholderFactory<PlayerController> { }

		[System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "Health" ), MinValue( 0 )]
			public float Health;
			[FoldoutGroup( "Health" ), HideLabel]
			public StunController.Settings Stun;
			[FoldoutGroup( "Health" ), HideLabel]
			public DamageHandlerController.Settings Damage;

			[FoldoutGroup( "Death" ), MinValue( 0 )]
			public float DeathAnimDuration;

			[FoldoutGroup( "Motor" ), HideLabel]
			public CharacterMotor.Settings Motor;
			[TitleGroup( "Rotation", GroupID = "Motor/Rotation" ), HideLabel]
			public TiltRotationMotor.Settings Rotation;
			[TitleGroup( "Dodging", GroupID = "Motor/Dodging" ), HideLabel]
			public DodgeController.Settings Dodge;

			[FoldoutGroup( "Weapons" )]
			public GunInstaller PrimaryWeaponPrefab;
			[FoldoutGroup( "Weapons" )]
			public GunInstaller SecondaryWeaponPrefab;

			[FoldoutGroup( "Camera" )]
			public TargetGroupAttachment.Settings PlayerTarget = new TargetGroupAttachment.Settings( "Player" );
			[FoldoutGroup( "Camera" )]
			public TargetGroupAttachment.Settings AimTarget = new TargetGroupAttachment.Settings( "Aim Offset" );
		}
	}
}

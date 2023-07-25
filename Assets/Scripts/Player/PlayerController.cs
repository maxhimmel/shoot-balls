using ShootBalls.Gameplay.Cameras;
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
		IDamageable
	{
		public Rigidbody2D Body => _body;

		private readonly Settings _settings;
		private readonly Rewired.Player _input;
		private readonly CharacterMotor _motor;
		private readonly IRotationMotor _rotation;
		private readonly DodgeController _dodgeController;
		private readonly Gun.Factory _gunFactory;
		private readonly Rigidbody2D _body;

		private Gun _primaryGun, _secondaryGun;

		public PlayerController( Settings settings,
			Rewired.Player input,
			CharacterMotor motor,
			IRotationMotor rotation,
			DodgeController dodgeController,
			Gun.Factory gunFactory,
			Rigidbody2D body )
		{
			_settings = settings;
			_input = input;
			_motor = motor;
			_rotation = rotation;
			_dodgeController = dodgeController;
			_gunFactory = gunFactory;
			_body = body;
		}

		public void Initialize()
		{
			_primaryGun = _gunFactory.Create( _settings.PrimaryWeaponPrefab );
			_secondaryGun = _gunFactory.Create( _settings.SecondaryWeaponPrefab );

			_primaryGun.SetOwner( this );
			_secondaryGun.SetOwner( this );
		}

		public void Tick()
		{
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
			_motor.FixedTick();
			_rotation.FixedTick();
		}

		void IPushable.Push( Vector2 velocity )
		{
			_body.AddForce( velocity, ForceMode2D.Impulse );
		}

		public bool TakeDamage( IDamageData data )
		{
			Debug.Log( $"Player damaged!\n" +
				$"<b>Instigator:</b> {data.Instigator.Body.name} | <b>Causer:</b> {data.Causer.Body.name}" );

			return false;
		}

		public class Factory : PlaceholderFactory<PlayerController> { }

		[System.Serializable]
		public class Settings
		{
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

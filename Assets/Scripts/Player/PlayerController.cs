using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Pawn;
using ShootBalls.Gameplay.Weapons;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Player
{
	public class PlayerController : IPawn,
		ITickable,
		IFixedTickable
	{
		public Rigidbody2D Body => _body;

		private readonly Rewired.Player _input;
		private readonly CharacterMotor _motor;
		private readonly Gun[] _guns;
		private readonly Rigidbody2D _body;

		public PlayerController( Rewired.Player input,
			CharacterMotor motor,
			Gun[] guns,
			Rigidbody2D body )
		{
			_input = input;
			_motor = motor;
			_guns = guns;
			_body = body;

			foreach ( var gun in guns )
			{
				gun.SetOwner( this );
			}
		}

		public void Tick()
		{
			_motor.SetDesiredVelocity( _input.GetClampedAxis2D( ReConsts.Action.Horizontal, ReConsts.Action.Vertical ) );

			var aimDirection = _input.GetClampedAxis2D( ReConsts.Action.AimHorizontal, ReConsts.Action.AimVertical );
			if ( aimDirection != Vector2.zero )
			{
				_motor.SetDesiredRotation( aimDirection );
			}

			HandleGunFiring();
		}

		private void HandleGunFiring()
		{
			if ( _input.GetButtonDown( ReConsts.Action.PrimaryFire ) )
			{
				_guns[0].StartFiring();
			}
			else if ( _input.GetButtonUp( ReConsts.Action.PrimaryFire ) )
			{
				_guns[0].StopFiring();
			}

			if ( _input.GetButtonDown( ReConsts.Action.SecondaryFire ) )
			{
				_guns[1].StartFiring();
			}
			else if ( _input.GetButtonUp( ReConsts.Action.SecondaryFire ) )
			{
				_guns[1].StopFiring();
			}
		}

		public void FixedTick()
		{
			_motor.FixedTick();
		}

		public class Factory : PlaceholderFactory<PlayerController> { }
	}
}

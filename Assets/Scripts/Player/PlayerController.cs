using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Weapons;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Player
{
	public class PlayerController : ITickable
	{
		private readonly Rewired.Player _input;
		private readonly CharacterMotor _motor;
		private readonly Gun _gun;

		public PlayerController( Rewired.Player input,
			CharacterMotor motor,
			Gun gun )
		{
			_input = input;
			_motor = motor;
			_gun = gun;
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
			if ( _input.GetButtonDown( ReConsts.Action.Fire ) )
			{
				_gun.StartFiring();
			}
			else if ( _input.GetButtonUp( ReConsts.Action.Fire ) )
			{
				_gun.StopFiring();
			}
		}
	}
}

using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Weapons;
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
			_motor.SetDesiredVelocity( GetMoveInput() );

			HandleGunFiring();
		}

		private Vector2 GetMoveInput()
		{
			var rawInput = _input.GetAxis2D( ReConsts.Action.Horizontal, ReConsts.Action.Vertical );
			return Vector2.ClampMagnitude( rawInput, 1 );
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

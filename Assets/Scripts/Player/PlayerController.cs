using ShootBalls.Movement;
using UnityEngine;
using Zenject;

namespace ShootBalls.Player
{
	public class PlayerController : ITickable
	{
		private readonly Rewired.Player _input;
		private readonly CharacterMotor _motor;

		public PlayerController( Rewired.Player input,
			CharacterMotor motor )
		{
			_input = input;
			_motor = motor;
		}

		public void Tick()
		{
			_motor.SetDesiredVelocity( GetMoveInput() );
		}

		private Vector2 GetMoveInput()
		{
			var rawInput = _input.GetAxis2D( ReConsts.Action.Horizontal, ReConsts.Action.Vertical );
			return Vector2.ClampMagnitude( rawInput, 1 );
		}
    }
}

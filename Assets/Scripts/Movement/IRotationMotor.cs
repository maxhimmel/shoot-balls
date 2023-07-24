using UnityEngine;

namespace ShootBalls.Gameplay.Movement
{
	public interface IRotationMotor
	{
		void SetDesiredRotation( Vector2 direction );

		void FixedTick();
	}
}
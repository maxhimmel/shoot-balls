using UnityEngine;

namespace ShootBalls.Gameplay.Pawn
{
	public interface IPawn
	{
		Rigidbody2D Body { get; }
	}
}
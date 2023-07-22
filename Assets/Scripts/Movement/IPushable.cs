using ShootBalls.Gameplay.Pawn;
using UnityEngine;

namespace ShootBalls.Gameplay
{
	public interface IPushable : IPawn
	{
		void Push( Vector2 velocity );
	}
}
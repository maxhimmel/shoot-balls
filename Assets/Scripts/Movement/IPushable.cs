using ShootBalls.Utility;
using UnityEngine;

namespace ShootBalls.Gameplay
{
	public interface IPushable
	{
		IOrientation Orientation { get; }

		void Push( Vector2 velocity );
	}
}
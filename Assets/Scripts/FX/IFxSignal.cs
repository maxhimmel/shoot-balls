using UnityEngine;

namespace ShootBalls.Gameplay.Fx
{
	public interface IFxSignal
	{
		Vector2 Position { get; }
		Vector2 Direction { get; }
		Transform Parent { get; }
	}

	public class FxSignal : IFxSignal
	{
		public Vector2 Position { get; set; }
		public Vector2 Direction { get; set; }
		public Transform Parent { get; set; }
	}
}

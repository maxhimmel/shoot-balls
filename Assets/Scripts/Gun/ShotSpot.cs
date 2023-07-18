using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public class ShotSpot
	{
		public Vector2 Position => _shotSpot.position;
		public Vector2 Facing => Rotation * Vector2.up;
		public Vector2 Tangent => Rotation * Vector2.right;
		public Quaternion Rotation => _shotSpot.rotation;
		public Transform Transform => _shotSpot;

		private readonly Transform _shotSpot;

		public ShotSpot( Transform shotSpot )
		{
			_shotSpot = shotSpot;
		}
	}
}

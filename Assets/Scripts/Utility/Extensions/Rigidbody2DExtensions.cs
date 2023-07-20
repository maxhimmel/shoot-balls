using UnityEngine;

namespace ShootBalls.Utility
{
	public static class Rigidbody2DExtensions
	{
		public static void AddExplosionForce( this Rigidbody2D self, 
			float explosionForce, 
			Vector2 explosionPosition, 
			float explosionRadius, 
			ForceMode2D mode )
		{
			if ( explosionForce == 0 || explosionRadius == 0 )
			{
				return;
			}

			Vector2 direction = self.position - explosionPosition;

			float distance = direction.magnitude;
			float distancePercent = distance / explosionRadius;

			float forceScalar = Mathf.Clamp01( 1 - distancePercent );
			Vector2 force = explosionForce * forceScalar * (direction / distance);

			self.AddForce( force, mode );
		}

		/// <summary>
		/// Moves the rigidbody's transform as well as updates the physics orientation.
		/// </summary>
		public static void HolisticMove( this Rigidbody2D self, Vector2 position, Quaternion rotation )
		{
			self.transform.SetPositionAndRotation( position, rotation );
			self.position = position;
			self.SetRotation( rotation );
		}
	}
}
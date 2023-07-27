using ShootBalls.Gameplay.Pawn;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Attacking
{
	public static class ExplosionController
	{
		private static readonly Collider2D[] _explosionBuffer = new Collider2D[100];

		public static void Explode( Request request )
		{
			int overlapCount = Physics2D.OverlapCircleNonAlloc(
				request.Source.Body.position, request.Settings.Radius, _explosionBuffer, request.Settings.Layer
			);

			for ( int idx = 0; idx < overlapCount; ++idx )
			{
				var explosion = _explosionBuffer[idx];
				if ( explosion.attachedRigidbody == null )
				{
					continue;
				}

				explosion.attachedRigidbody.AddExplosionForce(
					request.Settings.Force,
					request.Source.Body.position,
					request.Settings.Radius,
					ForceMode2D.Impulse
				);
			}
		}

		public class Request
		{
			public IPawn Source;
			public Settings Settings;
		}

		[System.Serializable]
		public class Settings
		{
			public LayerMask Layer;

			[HorizontalGroup, MinValue( 0 )]
			public float Radius;
			[HorizontalGroup]
			public float Force;
		}
	}
}
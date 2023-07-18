using System;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public class AngleDirectionAdjuster : IPreFireProcessor
	{
		protected readonly Settings _settings;

		public AngleDirectionAdjuster( Settings settings )
		{
			_settings = settings;
		}

		public void PreFire( ref IOrientation orientation )
		{
			float randAngle = GetAngle() * RandomExtensions.Sign();
			orientation.Rotation *= Quaternion.Euler( 0, 0, randAngle );
		}

		protected virtual float GetAngle()
		{
			return _settings.AngleRange.Random();
		}

		[System.Serializable]
		public class Settings : IGunModule
		{
			public virtual Type ModuleType => typeof( AngleDirectionAdjuster );

			[MinMaxSlider( 0, 180f, ShowFields = true )]
			public Vector2 AngleRange;
		}
	}
}

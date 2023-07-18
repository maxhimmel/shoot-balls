using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public class BurstFireSafety : IFireSafety,
		IFireEndProcessor,
		IGunTickable
	{
		private readonly Settings _settings;

		private int _fireCount;
		private float _delayEndTime;

		public BurstFireSafety( Settings settings )
		{
			_settings = settings;
		}

		public void FireEnding()
		{
			++_fireCount;
			if ( !CanFire() )
			{
				_delayEndTime = Time.timeSinceLevelLoad + _settings.Delay;
			}
		}

		public void FixedTick()
		{
			if ( CanFire() )
			{
				return;
			}
			if ( _delayEndTime > Time.timeSinceLevelLoad )
			{
				return;
			}

			_fireCount = 0;
		}

		public bool CanFire()
		{
			return _fireCount < _settings.Grouping;
		}

		[System.Serializable]
		public class Settings : IGunModule
		{
			public Type ModuleType => typeof( BurstFireSafety );

			[MinValue( 1 )]
			public int Grouping;

			[MinValue( 0 )]
			public float Delay;
		}
	}
}
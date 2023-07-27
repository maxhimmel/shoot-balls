using System;
using Sirenix.OdinInspector;

namespace ShootBalls.Gameplay.Fx
{
	public class HitStopVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly TimeScaleFxQueue _timeScaleFxQueue;

		public HitStopVfxAnimator( Settings settings,
			TimeScaleFxQueue timeScaleFxQueue )
		{
			_settings = settings;
			_timeScaleFxQueue = timeScaleFxQueue;
		}

		public void Play( IFxSignal signal )
		{
			_timeScaleFxQueue.AdjustForSeconds( new TimeScaleFxQueue.Request(
				_settings.Duration,
				_settings.TimeScale
			) );
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( HitStopVfxAnimator );

			[HorizontalGroup]
			public float Duration;
			[HorizontalGroup, MinValue( 0 )]
			public float TimeScale;
		}
	}
}

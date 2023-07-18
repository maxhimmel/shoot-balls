using System;
using Cysharp.Threading.Tasks;
using ShootBalls.Utility;

namespace ShootBalls.Gameplay.Fx
{
	public class HitStopVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly TimeController _timeController;

		public HitStopVfxAnimator( Settings settings,
			TimeController timeController )
		{
			_settings = settings;
			_timeController = timeController;
		}

		public void Play( IFxSignal signal )
		{
			_timeController.PauseForSeconds( _settings.Duration )
				.Forget();
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( HitStopVfxAnimator );

			public float Duration;
		}
	}
}

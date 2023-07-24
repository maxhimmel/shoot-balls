using System;
using ShootBalls.Gameplay.Audio;

namespace ShootBalls.Gameplay.Fx
{
	public class SfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly IAudioController _audioController;

		public SfxAnimator( Settings settings,
			IAudioController audioController )
		{
			_settings = settings;
			_audioController = audioController;
		}

		public void Play( IFxSignal signal )
		{
			_audioController.PlayOneShot( _settings.EventReference.EventName, signal.Position );
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( SfxAnimator );

			public AudioEventReference EventReference;
		}
	}
}
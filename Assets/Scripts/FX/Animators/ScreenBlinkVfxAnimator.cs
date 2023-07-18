using Sirenix.OdinInspector;

namespace ShootBalls.Gameplay.Fx
{
	public class ScreenBlinkVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly ScreenBlinkController _screenBlinker;

		public ScreenBlinkVfxAnimator( Settings settings, 
			ScreenBlinkController screenBlinker )
		{
			_settings = settings;
			_screenBlinker = screenBlinker;
		}

		public void Play( IFxSignal signal )
		{
			_screenBlinker.Blink( _settings.Blink );
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public System.Type AnimatorType => typeof( ScreenBlinkVfxAnimator );

			[HideLabel]
			public ScreenBlinkController.Settings Blink;
		}
	}
}
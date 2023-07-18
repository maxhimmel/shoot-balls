using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Fx
{
	public class SpriteBlinkVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly SpriteBlinker _blinker;

		public SpriteBlinkVfxAnimator( Settings settings,
			SpriteRenderer renderer )
		{
			_settings = settings;
			_blinker = new SpriteBlinker( renderer );
		}

		public void Play( IFxSignal signal )
		{
			_blinker.Blink( _settings.BlinkSettings )
				.Forget();
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( SpriteBlinkVfxAnimator );

			[HideLabel]
			public SpriteBlinker.Settings BlinkSettings;
		}
	}
}
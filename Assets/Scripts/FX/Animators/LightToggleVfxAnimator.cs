using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ShootBalls.Gameplay.Fx
{
	public class LightToggleVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly bool _initialEnabledState;

		private float _timer;

		public LightToggleVfxAnimator( Settings settings )
		{
			_settings = settings;
			_initialEnabledState = settings.Light.enabled;
		}

		public void Play( IFxSignal signal )
		{
			if ( _timer <= 0 )
			{
				Play().Forget();
			}
			else
			{
				_timer = _settings.Duration;
			}
		}

		private async UniTaskVoid Play()
		{
			_settings.Light.enabled = !_initialEnabledState;

			_timer = _settings.Duration;
			while ( _timer > 0 )
			{
				_timer -= Time.deltaTime;
				await UniTask.Yield( PlayerLoopTiming.Update );

				if ( _settings.Light == null )
				{
					return;
				}
			}

			_settings.Light.enabled = _initialEnabledState;
		}

		[System.Serializable]
		public class Settings : IFxAnimator.Settings<LightToggleVfxAnimator>
		{
			public Light2D Light;

			[HorizontalGroup]
			public bool IsEnabled;
			[HorizontalGroup, MinValue( 0 )]
			public float Duration;
		}
	}
}

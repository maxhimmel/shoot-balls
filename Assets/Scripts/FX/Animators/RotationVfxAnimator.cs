using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class RotationVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly Transform _renderer;
		private readonly Quaternion _initialLocalRotation;
		private readonly CancellationToken _destroyCancelToken;

		public RotationVfxAnimator( Settings settings,
			[Inject( Id = "Renderer" )] Transform renderer )
		{
			_settings = settings;
			_renderer = renderer;
			_initialLocalRotation = renderer.localRotation;
			_destroyCancelToken = renderer.GetCancellationTokenOnDestroy();
		}

		public void Play( IFxSignal signal )
		{
			Play().Cancellable( _destroyCancelToken );
		}

		private async UniTask Play()
		{
			float randAngle = _settings.AngleRange.Random();
			_renderer.localRotation *= Quaternion.Euler( 0, 0, randAngle );

			await TaskHelpers.DelaySeconds( _settings.Duration, _destroyCancelToken );

			_renderer.localRotation = _initialLocalRotation;
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( RotationVfxAnimator );

			[MinMaxSlider( -180, 180, ShowFields = true )]
			public Vector2 AngleRange;

			public float Duration;
		}
	}
}

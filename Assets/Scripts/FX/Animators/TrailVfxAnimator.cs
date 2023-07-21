using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Fx
{
	public class TrailVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;

		public TrailVfxAnimator( Settings settings )
		{
			_settings = settings;
		}

		public void Play( IFxSignal signal )
		{
			Play().Forget();
		}

		private async UniTaskVoid Play()
		{
			bool prevEmitState = _settings.Trail.emitting;
			_settings.Trail.emitting = _settings.IsEmitting;

			float timer = 0;
			while ( timer < _settings.Duration )
			{
				timer += Time.deltaTime;
				await UniTask.Yield( PlayerLoopTiming.Update );

				if ( _settings.Trail == null )
				{
					return;
				}
			}

			_settings.Trail.emitting = prevEmitState;
		}

		[System.Serializable]
		public class Settings : IFxAnimator.Settings<TrailVfxAnimator>
		{
			public TrailRenderer Trail;

			[HorizontalGroup]
			public bool IsEmitting;
			[HorizontalGroup, MinValue( 0 )]
			public float Duration;
		}
	}
}

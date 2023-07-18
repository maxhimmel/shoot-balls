using System.Threading;
using Cysharp.Threading.Tasks;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
    public class BounceScaler
    {
		private readonly Transform _renderer;
		private readonly Vector2 _initialScale;
		private readonly CancellationToken _destroyedCancelToken;

		public BounceScaler( [Inject( Id = "Renderer" )] Transform renderer )
		{
			_renderer = renderer;
			_initialScale = renderer.transform.localScale;
			_destroyedCancelToken = renderer.GetCancellationTokenOnDestroy();
		}

		public async UniTask Bounce( Settings data )
		{
			float timer = 0;
			float curveDuration = data.Curve.GetDuration();

			while ( timer < 1 )
			{
				timer += Time.deltaTime / data.Duration;

				float curveTime = Mathf.Lerp( 0, curveDuration, timer );
				float scalar = data.Curve.Evaluate( curveTime ) * data.Scale;
				_renderer.transform.localScale = _initialScale + Vector2.one * scalar;

				await UniTask.Yield( _destroyedCancelToken )
					.SuppressCancellationThrow();

				if ( _destroyedCancelToken.IsCancellationRequested )
				{
					return;
				}
			}

			if ( data.ResetScale )
			{
				_renderer.transform.localScale = _initialScale;
			}
		}

        [System.Serializable]
		public class Settings
		{
			public float Scale;
			public float Duration;
			public bool ResetScale = true;

			[InfoBox( "This is additively applied to the current scale." )]
			public AnimationCurve Curve;
		}
	}
}

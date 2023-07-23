using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ShootBalls.Gameplay.Fx
{
	public class LightRadiusVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;

		private float _timer;

		public LightRadiusVfxAnimator( Settings settings )
		{
			_settings = settings;
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
			_timer = _settings.Duration;
			while ( _timer > 0 )
			{
				_timer -= Time.deltaTime;

				float lerpValue = _settings.Animation.Evaluate( 1f - _timer / _settings.Duration );
				
				float inner = Mathf.LerpUnclamped( _settings.InnerStart, _settings.InnerEnd, lerpValue );
				float outer = Mathf.LerpUnclamped( _settings.OuterStart, _settings.OuterEnd, lerpValue );
				_settings.Light.pointLightInnerRadius = inner;
				_settings.Light.pointLightOuterRadius = outer;

				await UniTask.Yield( PlayerLoopTiming.Update );

				if ( _settings.Light == null )
				{
					return;
				}
			}
		}

		[System.Serializable]
		public class Settings : IFxAnimator.Settings<LightRadiusVfxAnimator>
		{
			public Light2D Light;

			[MinValue( 0 )]
			public float Duration;
			public AnimationCurve Animation = AnimationCurve.EaseInOut( 0, 0, 1, 1 );

			[HorizontalGroup( "Start" )]
			public float InnerStart;
			[HorizontalGroup( "Start" )]
			public float OuterStart;

			[HorizontalGroup( "End" )]
			public float InnerEnd;
			[HorizontalGroup( "End" )]
			public float OuterEnd;
		}
	}
}
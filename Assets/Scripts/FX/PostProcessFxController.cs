using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class PostProcessFxController : ITickable
	{
		private readonly Settings _settings;
		private readonly GlobalFxValue _globalFx;

		private float _chromaticVelocity;
		private float _chromaticDamping;
		private float _paniniVelocity;
		private float _paniniDamping;

		public PostProcessFxController( Settings settings,
			GlobalFxValue globalFx )
		{
			_settings = settings;
			_globalFx = globalFx;
		}

		public void Tick()
		{
			if ( _settings.Profile.TryGet<ChromaticAberration>( out var chromatic ) )
			{
				_chromaticDamping += _globalFx.AbsDelta;
				_chromaticDamping = Mathf.SmoothDamp( _chromaticDamping, 0, ref _chromaticVelocity, _settings.ChromaticSmoothDamp );

				float fx = _chromaticDamping * _settings.GlobalFxInfluence;

				chromatic.intensity.value = Mathf.Clamp(
					_settings.ChromaticRange.x + fx,
					_settings.ChromaticRange.x,
					_settings.ChromaticRange.y 
				);
			}

			if ( _settings.Profile.TryGet<PaniniProjection>( out var panini ) )
			{
				_paniniDamping += _globalFx.AbsDelta;
				_paniniDamping = Mathf.SmoothDamp( _paniniDamping, 0, ref _paniniVelocity, _settings.PaniniSmoothDamp );

				float fx = _paniniDamping * _settings.GlobalFxInfluence;

				panini.distance.value = Mathf.Clamp(
					_settings.PaniniRange.x + fx,
					_settings.PaniniRange.x,
					_settings.PaniniRange.y
				);
			}
		}

		[System.Serializable]
		public class Settings
		{
			[InlineEditor]
			public VolumeProfile Profile;
			public float GlobalFxInfluence;

			[FoldoutGroup( "Chromatic Aberration" ), MinMaxSlider( 0, 1, ShowFields = true )]
			public Vector2 ChromaticRange;
			[FoldoutGroup( "Chromatic Aberration" ), MinValue( 0 )]
			public float ChromaticSmoothDamp;

			[FoldoutGroup( "Panini Projection" ), MinMaxSlider( 0, 1, ShowFields = true )]
			public Vector2 PaniniRange;
			[FoldoutGroup( "Panini Projection" ), MinValue( 0 )]
			public float PaniniSmoothDamp;
		}
	}
}

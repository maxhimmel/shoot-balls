using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ShootBalls.Gameplay.Fx
{
	public class ChromaticAberrationFxProcessor : PostProcessFxProcessor<ChromaticAberration>
	{
		private readonly Settings _settings;

		private float _initialIntensity;

		public ChromaticAberrationFxProcessor( Settings settings )
		{
			_settings = settings;
		}

		protected override PostProcessFxProcessor<ChromaticAberration>.Settings GetSettings()
		{
			return _settings;
		}

		protected override void ProcessFx( ChromaticAberration component, float fx )
		{
			component.intensity.value = Mathf.Clamp(
				_settings.IntensityRange.x + fx,
				_settings.IntensityRange.x,
				_settings.IntensityRange.y
			);
		}

		protected override void CachePostProcessProfileDefaults( ChromaticAberration component )
		{
			_initialIntensity = component.intensity.value;
		}

		protected override void RestorePostProcessProfileFields( ChromaticAberration component )
		{
			component.intensity.value = _initialIntensity;
		}

		[System.Serializable]
		public new class Settings : PostProcessFxProcessor<ChromaticAberration>.Settings
		{
			public override Type ProcessorType => typeof( ChromaticAberrationFxProcessor );

			[MinMaxSlider( 0, 1, ShowFields = true )]
			public Vector2 IntensityRange;

			protected override object GetBindingArgs()
			{
				return this;
			}
		}
	}
}
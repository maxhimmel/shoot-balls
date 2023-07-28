using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ShootBalls.Gameplay.Fx
{
	public class PaniniProjectionFxProcessor : PostProcessFxProcessor<PaniniProjection>
	{
		private readonly Settings _settings;

		private float _initialDistance;

		public PaniniProjectionFxProcessor( Settings settings )
		{
			_settings = settings;
		}

		protected override PostProcessFxProcessor<PaniniProjection>.Settings GetSettings()
		{
			return _settings;
		}

		protected override void ProcessFx( PaniniProjection component, float fx )
		{
			component.distance.value = Mathf.Clamp(
				_settings.DistanceRange.x + fx,
				_settings.DistanceRange.x,
				_settings.DistanceRange.y
			);
		}

		protected override void CachePostProcessProfileDefaults( PaniniProjection component )
		{
			_initialDistance = component.distance.value;
		}

		protected override void RestorePostProcessProfileFields( PaniniProjection component )
		{
			component.distance.value = _initialDistance;
		}

		[System.Serializable]
		public new class Settings : PostProcessFxProcessor<PaniniProjection>.Settings
		{
			public override Type ProcessorType => typeof( PaniniProjectionFxProcessor );

			[MinMaxSlider( 0, 1, ShowFields = true )]
			public Vector2 DistanceRange;

			protected override object GetBindingArgs()
			{
				return this;
			}
		}
	}
}
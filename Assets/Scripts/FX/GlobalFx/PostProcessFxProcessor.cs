using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace ShootBalls.Gameplay.Fx
{
	public abstract class PostProcessFxProcessor<TVolumeComponent> : IGlobalFxProcessor
		where TVolumeComponent : VolumeComponent
	{
		protected float _velocity;
		protected float _damping;

		public void Tick( GlobalFxValue globalFx )
		{
			var settings = GetSettings();
			if ( settings.Influence == 0 )
			{
				return;
			}

			if ( settings.Profile.TryGet<TVolumeComponent>( out var component ) )
			{
				_damping += globalFx.AbsDelta;
				_damping = Mathf.SmoothDamp( _damping, 0, ref _velocity, settings.SmoothDamp );

				ProcessFx( component, _damping * settings.Influence );
			}
		}

		protected abstract Settings GetSettings();

		protected abstract void ProcessFx( TVolumeComponent component, float fx );

		public class Settings : IGlobalFxProcessor.Settings<PostProcessFxProcessor<TVolumeComponent>>
		{
			public override System.Type ProcessorType => throw new System.NotImplementedException();

			[InlineEditor]
			public VolumeProfile Profile;

			[HorizontalGroup, FormerlySerializedAs( "Weight" )]
			public float Influence;
			[HorizontalGroup]
			public float SmoothDamp;

			protected override object GetBindingArgs()
			{
				throw new System.NotImplementedException();
			}
		}
	}
}
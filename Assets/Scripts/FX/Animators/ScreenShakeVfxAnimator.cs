using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Fx
{
	public class ScreenShakeVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;

		public ScreenShakeVfxAnimator( Settings settings )
		{
			_settings = settings;
		}

		public void Play( IFxSignal signal )
		{
			Vector3 direction = signal.Direction;
			if ( direction == Vector3.zero )
			{
				direction = Vector3.up;
			}

			_settings.Definition.CreateEvent( signal.Position, direction * _settings.Force );
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( ScreenShakeVfxAnimator );

			[BoxGroup, DrawWithUnity]
			public CinemachineImpulseDefinition Definition;
			[BoxGroup]
			public float Force;
		}
	}
}
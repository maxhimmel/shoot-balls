using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Cameras
{
	public class TargetGroupAttachment : IInitializable,
		IDisposable
    {
		private readonly Settings _settings;
		private readonly CinemachineTargetGroup _targetGroup;

		public TargetGroupAttachment( Settings settings,
			CinemachineTargetGroup targetGroup )
		{
			_settings = settings;
			_targetGroup = targetGroup;
		}

		public void Dispose()
		{
			_targetGroup.RemoveMember( _settings.Focus );
		}

		public void Initialize()
		{
			_targetGroup.AddMember( _settings.Focus, _settings.Weight, _settings.Radius );
		}

        [System.Serializable]
		public class Settings
		{
			public Transform Focus;

			[HorizontalGroup, MinValue( 0 )]
			public float Weight;
			[HorizontalGroup, MinValue( 0 )]
			public float Radius;
		}
	}
}

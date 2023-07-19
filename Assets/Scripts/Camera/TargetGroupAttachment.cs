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

		[HideLabel]
        [System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "$_label" )]
			public Transform Focus;

			[HorizontalGroup( "$_label/Group" ), MinValue( 0 )]
			public float Weight;
			[HorizontalGroup( "$_label/Group" ), MinValue( 0 )]
			public float Radius;

			private readonly string _label;

			public Settings( string label )
			{
				_label = label;
			}
		}
	}
}

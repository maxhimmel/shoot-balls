using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Fx
{
	public class GlobalFxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly GlobalFxValue _globalFx;

		public GlobalFxAnimator( Settings settings,
			GlobalFxValue globalFx )
		{
			_settings = settings;
			_globalFx = globalFx;
		}

		public void Play( IFxSignal signal )
		{
			_globalFx.Add( new GlobalFxValue.Request()
			{
				Force = _settings.Force,
				Deceleration = _settings.Deceleration
			} );
		}

		[System.Serializable]
		public class Settings : IFxAnimator.Settings<GlobalFxAnimator>
		{
			[InfoBox( "@GetDurationMessage()", Icon = SdfIconType.ClockHistory )]

			public float Force;
			[MinValue( 0.01f )]
			public float Deceleration;

			private string GetDurationMessage()
			{
				return $"{Mathf.Abs( Force / Deceleration )} seconds long.";
			}
		}
	}
}

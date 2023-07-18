using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public class AccuracyAdjuster : AngleDirectionAdjuster,
		IFireEndProcessor,
		IGunTickable
	{
		private float _currentRecoil;
		private float _endFireRateTime;

		public AccuracyAdjuster( Settings settings ) 
			: base( settings )
		{
		}

		public void FireEnding()
		{
			var settings = GetSettings();

			_currentRecoil = Mathf.MoveTowards( _currentRecoil, 1, 1f / settings.ShotsToFullRecoil );
			_endFireRateTime = Time.timeSinceLevelLoad + settings.FireRate;
		}

		public void FixedTick()
		{
			if ( _endFireRateTime > Time.timeSinceLevelLoad )
			{
				return;
			}

			_currentRecoil = Mathf.MoveTowards( _currentRecoil, 0, Time.fixedDeltaTime / GetSettings().RebalanceDuration );
		}

		protected override float GetAngle()
		{
			float baseAngle = base.GetAngle();

			float recoilScale = Mathf.Clamp01( _currentRecoil );
			if ( Mathf.Approximately( recoilScale, 0 ) )
			{
				return baseAngle;
			}

			float recoilAngle = UnityEngine.Random.Range( 0, GetSettings().MaxAngleOverRecoil.Evaluate( recoilScale ) );
			return baseAngle + recoilAngle;
		}

		private Settings GetSettings()
		{
			return _settings as Settings;
		}

		[System.Serializable]
		public new class Settings : AngleDirectionAdjuster.Settings
		{
			public override Type ModuleType => typeof( AccuracyAdjuster );

			[Tooltip( "X: Recoil Ratio | Y: Max Angle" )]
			public AnimationCurve MaxAngleOverRecoil;

			[BoxGroup( "Recoil" ), MinValue( 1 )]
			public int ShotsToFullRecoil;
			[BoxGroup( "Recoil" )]
			public float RebalanceDuration;
			[BoxGroup( "Recoil" )]
			public float FireRate;
		}
	}
}
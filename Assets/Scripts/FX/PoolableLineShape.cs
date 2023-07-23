using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class PoolableLineShape : PoolableFx
	{
		private Settings MySettings => _settings as Settings;

		private Line _line;

		private float _timer;
		private float _growTimer;
		private Vector2 _endPoint;

		[Inject]
		public void Construct( Line line )
		{
			_line = line;
		}

		protected override void Play( IFxSignal signal )
		{
			_timer = MySettings.Duration;
			_growTimer = 0;
			_endPoint = signal.Position + signal.Direction;

			transform.SetPositionAndRotation( Vector3.zero, Quaternion.identity );

			_line.Start = _line.End = signal.Position;
		}

		protected override bool Tick()
		{
			var settings = MySettings;

			_timer -= settings.UseUnscaledTime 
				? Time.unscaledDeltaTime
				: Time.deltaTime;

			_growTimer += settings.UseUnscaledTime
				? Time.unscaledDeltaTime
				: Time.deltaTime;

			if ( _growTimer >= settings.GrowDuration )
			{
				_line.End = _endPoint;
			}
			else
			{
				_line.End = Vector2.LerpUnclamped( _line.Start, _endPoint, _growTimer / settings.GrowDuration );
			}

			_line.Color = Color.Lerp( settings.StartColor, settings.EndColor, 1f - _timer / settings.Duration );

			return _timer > 0;
		}

		[System.Serializable]
		public new class Settings : PoolableFx.Settings
		{
			public bool UseUnscaledTime;

			[MinValue( 0 )]
			public float Duration;

			[MinValue( 0 )]
			public float GrowDuration;

			public Color StartColor;
			public Color EndColor;
		}
	}
}
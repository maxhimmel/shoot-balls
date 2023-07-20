using System;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class ScreenColorShifter : ITickable
	{
		private readonly Settings _settings;
		private readonly ScreenBlinkController _screenBlinkController;

		private float _timer;
		private Color _startColor;
		private Color _nextColor;

		public ScreenColorShifter( Settings settings,
			ScreenBlinkController screenBlinkController )
		{
			_settings = settings;
			_screenBlinkController = screenBlinkController;

			_startColor = screenBlinkController.BaseColor;
			_nextColor = settings.Range.Generate();
		}

		public void Tick()
		{
			_timer += Time.deltaTime / _settings.DurationPerColor;

			var newColor = Color.Lerp( _startColor, _nextColor, _timer );
			_screenBlinkController.SetBaseColor( newColor );

			if ( _timer >= 1 )
			{
				_timer %= 1;
				_startColor = _nextColor;
				_nextColor = _settings.Range.Generate();
			}
		}

		[System.Serializable]
		public class Settings
		{
			[MinValue( float.Epsilon )]
			public float DurationPerColor = 3;
			[BoxGroup( "Color Range" ), HideLabel]
			public RandomColorSettings Range;
		}
	}
}
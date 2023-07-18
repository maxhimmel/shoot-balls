using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
    public class ScreenBlinkController : ITickable
	{
		private readonly Camera _camera;
		private readonly List<BlinkData> _blinks;

		private Color _baseColor;

		public ScreenBlinkController( Camera camera )
		{
			_camera = camera;
			_blinks = new List<BlinkData>();

			SetBaseColor( camera.backgroundColor );
		}

		public void SetBaseColor( Color color )
		{
			_baseColor = color;
		}

		public void Blink( Settings settings )
		{
			_blinks.Add( new BlinkData( settings.Color, settings.Duration ) );
		}

		public void Tick()
		{
			Color sum = _baseColor;

			for ( int idx = _blinks.Count - 1; idx >= 0; --idx )
			{
				var blink = _blinks[idx];
				sum += blink.Evaluate( Color.clear, out bool completed );

				if ( completed )
				{
					_blinks.RemoveAt( idx );
				}
			}

			_camera.backgroundColor = sum;
		}

		private class BlinkData
		{
			public Color Color { get; }
			public float Duration { get; }

			private float _timer;

			public BlinkData( Color color, float duration )
			{
				Color = color;
				Duration = duration;
			}

			public Color Evaluate( Color blendTo, out bool isComplete )
			{
				_timer += Time.deltaTime / Duration;
				_timer = Mathf.Min( _timer, 1 );

				isComplete = _timer >= 1;
				return Color.LerpUnclamped( Color, blendTo, _timer );
			}
		}

		[System.Serializable]
		public class Settings
		{
			public Color Color;
			public float Duration;
		}
	}
}

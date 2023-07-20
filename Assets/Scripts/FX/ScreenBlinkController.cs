using System.Collections.Generic;
using ShootBalls.Utility;
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
			_blinks.Add( new BlinkData( settings.Color, settings.Duration, settings.Operation ) );
		}

		public void Tick()
		{
			Color result = _baseColor;

			for ( int idx = _blinks.Count - 1; idx >= 0; --idx )
			{
				var blink = _blinks[idx];

				bool isCompleted = false;
				switch ( blink.Operation )
				{
					case Operation.Add:
						result += blink.Evaluate( Color.clear, out isCompleted );
						break;
					case Operation.Subtract:
						result -= blink.Evaluate( Color.clear, out isCompleted );
						break;
					case Operation.Multiply:
						result *= blink.Evaluate( Color.white, out isCompleted );
						break;
					case Operation.Divide:
						result = result.Divide( blink.Evaluate( Color.white, out isCompleted ) );
						break;
				}

				if ( isCompleted )
				{
					_blinks.RemoveAt( idx );
				}
			}

			_camera.backgroundColor = result;
		}

		private class BlinkData
		{
			public Color Color { get; }
			public float Duration { get; }
			public Operation Operation { get; }

			private float _timer;

			public BlinkData( Color color, float duration, Operation operation )
			{
				Color = color;
				Duration = duration;
				Operation = operation;
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
			public Operation Operation;
		}

		public enum Operation
		{
			Add,
			Subtract,
			Multiply,
			Divide
		}
	}
}

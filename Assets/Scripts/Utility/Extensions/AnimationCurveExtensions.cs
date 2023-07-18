using UnityEngine;

namespace ShootBalls.Utility
{
	public static class AnimationCurveExtensions
	{
		public static float GetDuration( this AnimationCurve curve )
		{
			int lastIndex = curve.length - 1;
			Keyframe lastKey = curve[lastIndex];

			return lastKey.time;
		}
	}
}
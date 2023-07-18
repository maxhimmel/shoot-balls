using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public interface IAmmoHandler : IFireSafety,
		IFireEndProcessor
	{
		event System.Action Emptied;

		AmmoData AmmoData { get; }

		void Reload();
	}

	public class AmmoData
	{
		public static readonly AmmoData Empty = new AmmoData( 0, 1 );
		public static readonly AmmoData Full = new AmmoData( 1, 1 );

		public float Normalized => Mathf.Clamp01( CurrentClip / MaxClipSize );

		public float CurrentClip;
		public float MaxClipSize;

		public AmmoData() { }
		public AmmoData( float currentClip, float maxClip )
		{
			CurrentClip = currentClip;
			MaxClipSize = maxClip;
		}
	}
}
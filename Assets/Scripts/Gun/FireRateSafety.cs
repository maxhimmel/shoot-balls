using System;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public class FireRateSafety : IFireSafety,
		IProjectileFiredProcessor,
		IGunTickable
	{
		public delegate void UpdateFireRateCooldown( float normalizedCooldown );
		public event UpdateFireRateCooldown FireRateCooldownUpdated;

		private readonly Settings _settings;

		private float _nextFireTime;
		private bool _sentCompletedSignal;

		public FireRateSafety( Settings settings )
		{
			_settings = settings;
		}

		public void Notify( Projectile firedProjectile )
		{
			_sentCompletedSignal = false;
			_nextFireTime = Time.timeSinceLevelLoad + _settings.FireRate;
		}

		public void FixedTick()
		{
			if ( !CanFire() )
			{
				SendFireRateSignal();
			}
			else if ( !_sentCompletedSignal )
			{
				_sentCompletedSignal = true;
				SendFireRateSignal();
			}
		}

		public bool CanFire()
		{
			return _nextFireTime <= Time.timeSinceLevelLoad;
		}

		private void SendFireRateSignal()
		{
			float remainingTime = _nextFireTime - Time.timeSinceLevelLoad;

			FireRateCooldownUpdated?.Invoke( Mathf.Clamp01( 1 - remainingTime / _settings.FireRate ) );
		}

		[System.Serializable]
		public class Settings : IGunModule
		{
			public Type ModuleType => typeof( FireRateSafety );

			public float FireRate;
		}
	}
}

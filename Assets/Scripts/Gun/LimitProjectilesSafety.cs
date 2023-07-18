using System;
using System.Collections.Generic;

namespace ShootBalls.Gameplay.Weapons
{
	public class LimitProjectilesSafety : IFireSafety,
		IProjectileFiredProcessor
	{
		private readonly Settings _settings;
		private readonly HashSet<Projectile> _livingProjectiles;

		public LimitProjectilesSafety( Settings settings )
		{
			_settings = settings;
			_livingProjectiles = new HashSet<Projectile>();
		}

		public bool CanFire()
		{
			return _livingProjectiles.Count < _settings.MaxProjectiles;
		}

		public void Notify( Projectile firedProjectile )
		{
			firedProjectile.Disposed += OnProjectileDestroyed;
			_livingProjectiles.Add( firedProjectile );
		}

		private void OnProjectileDestroyed( Projectile projectile )
		{
			projectile.Disposed -= OnProjectileDestroyed;
			if ( !_livingProjectiles.Remove( projectile ) )
			{
				throw new System.DataMisalignedException();
			}
		}

		[System.Serializable]
		public class Settings : IGunModule
		{
			public Type ModuleType => typeof( LimitProjectilesSafety );

			public int MaxProjectiles;
		}
	}
}

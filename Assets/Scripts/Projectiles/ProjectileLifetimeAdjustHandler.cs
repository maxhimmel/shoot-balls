using Sirenix.OdinInspector;

namespace ShootBalls.Gameplay.Weapons
{
	public class ProjectileLifetimeAdjustHandler : IProjectileDamageHandler
	{
		private readonly Settings _settings;

		public ProjectileLifetimeAdjustHandler( Settings settings )
		{
			_settings = settings;
		}

		public void Handle( Projectile projectile, DamageDeliveredSignal signal )
		{
			if ( projectile.Lifetimer.Countdown > _settings.AdjustedLifetime )
			{
				projectile.Lifetimer.SetLifetime( _settings.AdjustedLifetime );
			}
		}

		public void Dispose()
		{
			// ...
		}

		[System.Serializable]
		public class Settings : IProjectileDamageHandler.Settings<ProjectileLifetimeAdjustHandler>
		{
			[MinValue( 0 )]
			public float AdjustedLifetime;
		}
	}
}
namespace ShootBalls.Gameplay.Weapons
{
	public class ProjectileDisableCollisionHandler : IProjectileDamageHandler
	{
		private readonly Settings _settings;

		private Projectile _projectile;

		public ProjectileDisableCollisionHandler( Settings settings )
		{
			_settings = settings;
		}

		public void Handle( Projectile projectile, DamageDeliveredSignal signal )
		{
			_projectile = projectile;
			_projectile.SetCollisionActive( false );
		}

		public void Dispose()
		{
			_projectile?.SetCollisionActive( true );
		}

		[System.Serializable]
		public class Settings : IProjectileDamageHandler.Settings<ProjectileDisableCollisionHandler>
		{

		}
	}
}
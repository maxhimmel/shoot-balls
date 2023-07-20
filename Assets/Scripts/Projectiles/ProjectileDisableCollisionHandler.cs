using ShootBalls.Gameplay.Pawn;

namespace ShootBalls.Gameplay.Weapons
{
	public class ProjectileDisableCollisionHandler : ProjectileCollisionHandler
	{
		private Projectile _projectile;

		public ProjectileDisableCollisionHandler( Settings settings )
		{
		}

		protected override bool Handle( Projectile owner, IDamageData data )
		{
			_projectile = owner;
			_projectile.SetCollisionActive( false );

			return true;
		}

		public override void Dispose()
		{
			_projectile?.SetCollisionActive( true );
		}

		[System.Serializable]
		public class Settings : ProjectileDamageData<ProjectileDisableCollisionHandler>
		{

		}
	}
}
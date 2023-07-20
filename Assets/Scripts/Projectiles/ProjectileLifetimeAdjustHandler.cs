using ShootBalls.Gameplay.Pawn;
using Sirenix.OdinInspector;

namespace ShootBalls.Gameplay.Weapons
{
	public class ProjectileLifetimeAdjustHandler : ProjectileCollisionHandler
	{
		private readonly Settings _settings;

		public ProjectileLifetimeAdjustHandler( Settings settings )
		{
			_settings = settings;
		}

		protected override bool Handle( Projectile owner, IDamageData data )
		{
			if ( owner.Lifetimer.Countdown > _settings.AdjustedLifetime )
			{
				owner.Lifetimer.SetLifetime( _settings.AdjustedLifetime );
				return true;
			}

			return false;
		}

		public override void Dispose()
		{
			// ...
		}

		[System.Serializable]
		public class Settings : ProjectileDamageData<ProjectileLifetimeAdjustHandler>
		{
			[MinValue( 0 )]
			public float AdjustedLifetime;
		}
	}
}
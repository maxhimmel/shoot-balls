using ShootBalls.Gameplay.Pawn;
using Sirenix.OdinInspector;

namespace ShootBalls.Gameplay.Weapons
{
	public class ProjectilePierceHandler : ProjectileCollisionHandler
	{
		private readonly Settings _settings;

		private int _pierceCount;

		public ProjectilePierceHandler( Settings settings )
		{
			_settings = settings;
		}

		protected override bool Handle( Projectile owner, IDamageData data )
		{
			if ( _settings.IsUnlimited )
			{
				return false;
			}

			if ( _pierceCount++ >= _settings.Depth )
			{
				owner.Dispose();
			}

			return true;
		}

		public override void Dispose()
		{
			_pierceCount = 0;
		}

		[System.Serializable]
		public class Settings : ProjectileDamageData<ProjectilePierceHandler>
		{
			[HorizontalGroup]
			public bool IsUnlimited;

			[HorizontalGroup]
			[MinValue( 0 ), DisableIf( "IsUnlimited" )]
			public int Depth;
		}
	}
}
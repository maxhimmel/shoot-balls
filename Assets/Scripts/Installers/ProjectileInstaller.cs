using ShootBalls.Gameplay;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class ProjectileInstaller : Installer<ProjectileInstaller>
	{
		private readonly Projectile.Settings _settings;

		public ProjectileInstaller( Projectile.Settings settings )
		{
			_settings = settings;
		}

		public override void InstallBindings()
		{
			Container.Bind<Projectile>()
				.AsSingle()
				.WithArguments( _settings );

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();
		}
	}
}
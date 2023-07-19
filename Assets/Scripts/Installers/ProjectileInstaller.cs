using ShootBalls.Gameplay;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class ProjectileInstaller : Installer<ProjectileInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<Projectile>()
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();
		}
	}
}
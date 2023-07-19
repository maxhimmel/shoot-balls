using ShootBalls.Gameplay;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class BallInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<Ball>()
				.AsSingle()
				.NonLazy();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();
		}
	}
}

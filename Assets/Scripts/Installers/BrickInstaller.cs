using ShootBalls.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class BrickInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.Bind<Brick>()
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponentInChildren<Rigidbody2D> )
				.AsSingle();
		}
	}
}

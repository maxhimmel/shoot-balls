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

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();

			Container.Bind<Transform>()
				.WithId( "Renderer" )
				.FromResolveGetter<SpriteRenderer>( renderer => renderer.transform )
				.AsSingle();
		}
	}
}

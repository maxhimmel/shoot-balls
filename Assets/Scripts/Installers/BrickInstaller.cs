using ShootBalls.Gameplay.LevelPieces;
using ShootBalls.Gameplay.Pawn;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class BrickInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Brick>()
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponentInChildren<Rigidbody2D> )
				.AsSingle();

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();

			Container.Bind<Transform>()
				.WithId( "Renderer" )
				.FromResolveGetter<SpriteRenderer>( renderer => renderer.transform )
				.AsSingle();

			Container.BindInterfacesTo<StunDamageHandler>()
				.AsSingle();
		}
	}
}

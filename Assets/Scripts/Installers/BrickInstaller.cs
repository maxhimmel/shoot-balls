using ShootBalls.Gameplay.LevelPieces;
using ShootBalls.Gameplay.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class BrickInstaller : MonoInstaller
    {
		[BoxGroup( "Brick" ), HideLabel]
		[SerializeField] private Brick.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Brick>()
				.AsSingle()
				.WithArguments( _settings );

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

			Container.Bind<StunController>()
				.AsSingle()
				.WithArguments( _settings.Stun );
		}
	}
}

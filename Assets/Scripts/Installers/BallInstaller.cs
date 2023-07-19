using ShootBalls.Gameplay;
using ShootBalls.Gameplay.Cameras;
using ShootBalls.Gameplay.Movement;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class BallInstaller : MonoInstaller
    {
		[HideLabel]
		[SerializeField] private Ball.Settings _settings;

		[FoldoutGroup( "Movement" ), HideLabel]
		[SerializeField] private CharacterMotor.Settings _motor;

		[SerializeField] private TargetGroupAttachment.Settings _cameraTarget;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Ball>()
				.AsSingle()
				.WithArguments( _settings );

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

			Container.Bind<CharacterMotor>()
				.AsSingle()
				.WithArguments( _motor );

			Container.BindInterfacesAndSelfTo<TargetGroupAttachment>()
				.AsCached()
				.WithArguments( _cameraTarget );
		}
	}
}

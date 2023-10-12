using ShootBalls.Gameplay;
using ShootBalls.Gameplay.Attacking;
using ShootBalls.Gameplay.Cameras;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Pawn;
using ShootBalls.Gameplay.UI;
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

		[SerializeField] private TargetGroupAttachment.Settings _cameraTarget = new TargetGroupAttachment.Settings( "Cam Target" );

		[FoldoutGroup( "UI" )]
		[SerializeField] private StunWidget _stunWidgetPrefab;

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

			Container.BindInterfacesTo<StunDamageHandler>()
				.AsCached();

			Container.Bind<AttackController>()
				.AsSingle();

			StunInstaller.Install( Container, _settings.Stun, _stunWidgetPrefab );

			Container.Bind<DamageHandlerController>()
				.AsSingle()
				.WithArguments( _settings.Damage );

			Container.Bind<BrickHomingAdjuster>()
				.AsSingle()
				.WithArguments( _settings.BrickHoming );
		}
	}
}

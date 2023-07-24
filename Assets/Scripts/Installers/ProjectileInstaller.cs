using ShootBalls.Gameplay;
using ShootBalls.Gameplay.Attacking;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Weapons;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class ProjectileInstaller : MonoInstaller
	{
		[BoxGroup( "Ball Detection" )]
		[SerializeField] private OnTriggerEnter2DBroadcaster _ballEnterDetector;
		[BoxGroup( "Ball Detection" )]
		[SerializeField] private OnTriggerExit2DBroadcaster _ballExitDetector;
		[FoldoutGroup( "Ball Detection/Movement" ), HideLabel]
		[SerializeField] private CharacterMotor.Settings _movement;
		[FoldoutGroup( "Ball Detection/Movement" ), HideLabel]
		[SerializeField] private RotationMotor.Settings _rotation;

		[SerializeReference] private IProjectileDamageData[] _collisionReactions = new IProjectileDamageData[0];

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Projectile>()
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Collider2D>()
				.FromMethod( GetComponentInChildren<Collider2D> )
				.AsSingle();

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();

			Container.Bind<Transform>()
				.WithId( "Renderer" )
				.FromResolveGetter<SpriteRenderer>( renderer => renderer.transform )
				.AsSingle();

			foreach ( var settings in _collisionReactions )
			{
				Container.Bind<ProjectileCollisionHandler>()
					.To( settings.HandlerType )
					.AsCached()
					.WithArguments( settings );
			}

			Container.BindInstances( _ballEnterDetector, _ballExitDetector );

			Container.Bind<CharacterMotor>()
				.AsSingle()
				.WithArguments( _movement );

			Container.BindInterfacesTo<RotationMotor>()
				.AsSingle()
				.WithArguments( _rotation );

			Container.Bind<AttackController>()
				.AsSingle();
		}
	}
}
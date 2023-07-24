using ShootBalls.Gameplay.Cameras;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Player;
using ShootBalls.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class PlayerInstaller : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private PlayerController.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<PlayerController>()
				.AsSingle()
				.WithArguments( _settings );

			/* --- */

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

			/* --- */

			Container.Bind<CharacterMotor>()
				.AsSingle()
				.WithArguments( _settings.Motor );

			Container.BindInterfacesTo<TiltRotationMotor>()
				.AsSingle()
				.WithArguments( _settings.Rotation );

			Container.Bind<DodgeController>()
				.AsSingle()
				.WithArguments( _settings.Dodge );

			/* --- */

			Container.BindInterfacesAndSelfTo<TargetGroupAttachment>()
				.AsCached()
				.WithArguments( _settings.PlayerTarget );

			Container.BindInterfacesAndSelfTo<TargetGroupAttachment>()
				.AsCached()
				.WithArguments( _settings.AimTarget );

			/* --- */

			Container.BindFactory<GunInstaller, Gun, Gun.Factory>()
				.FromFactory<Gun.CustomFactory>();
		}
	}
}

using ShootBalls.Gameplay;
using ShootBalls.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class GunInstaller : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private Gun.Settings _gun = new Gun.Settings();

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<Gun>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<Gun>()
						.AsSingle()
						.WithArguments( _gun );

					BindProjectile( subContainer );

					BindModules( subContainer );
				} )
				.AsCached();
		}

		private void BindProjectile( DiContainer subContainer )
		{
			subContainer.BindFactory<Projectile.Settings, Projectile, Projectile.Factory>()
				.FromPoolableMemoryPool( pool => pool
					.FromSubContainerResolve()
					.ByNewContextPrefab( _gun.ProjectilePrefab )
					.WithGameObjectName( _gun.ProjectilePrefab.name )
					.UnderTransform( context => context.Container.ResolveId<Transform>( _gun.ProjectilePoolId ) )
				);
		}

		private void BindModules( DiContainer subContainer )
		{
			subContainer.Bind<ShotSpot>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<ShotSpot>()
						.AsSingle();

					subContainer.BindInstance( _gun.ShotSpot );
				} )
				.AsSingle();

			/* --- */

			subContainer.BindInterfacesTo( _gun.FireSpread.ModuleType )
				.AsSingle()
				.WithArguments( _gun.FireSpread );

			foreach ( var settings in _gun.Modules )
			{
				subContainer.BindInterfacesAndSelfTo( settings.ModuleType )
					.AsCached()
					.WithArguments( settings );
			}
		}
	}
}

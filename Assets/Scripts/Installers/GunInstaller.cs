using ShootBalls.Gameplay;
using ShootBalls.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class GunInstaller : MonoInstaller
	{
		//[FoldoutGroup( "Damage" ), HideLabel]
		//[SerializeField] private DamageTrigger.Settings _damage;

		[FoldoutGroup( "Gun" ), HideLabel]
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

					//subContainer.BindInstance( _damage );

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

					subContainer.BindFactory<Projectile.Settings, Projectile, Projectile.Factory>()
						.FromSubContainerResolve()
						.ByNewPrefabInstaller<ProjectileInstaller>( _gun.ProjectilePrefab )
						.UnderTransform( context => null );

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
				} )
				.AsSingle();
		}
	}
}

using ShootBalls.Gameplay.Pawn;
using ShootBalls.Gameplay.UI;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class StunInstaller : Installer<StunController.Settings, StunWidget, StunInstaller>
	{
		[Inject] 
		private readonly StunController.Settings _settings;

		[InjectOptional] 
		private readonly StunWidget _stunWidgetPrefab;

		public override void InstallBindings()
		{
			Container.Bind<StunModel>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<StunModel>()
						.AsSingle();

					if ( _stunWidgetPrefab != null )
					{
						subContainer.Bind<StunWidget>()
							.FromComponentInNewPrefab( _stunWidgetPrefab )
							.WithGameObjectName( _stunWidgetPrefab.name )
							.UnderTransform( GetTransform )
							.AsSingle()
							.NonLazy();
					}
				} )
				.AsSingle();

			Container.BindInterfacesAndSelfTo<StunController>()
				.AsSingle()
				.WithArguments( _settings );
		}

		private Transform GetTransform( InjectContext context )
		{
			return context.Container.TryResolveId<Transform>( "Renderer" ) ?? context.Container.DefaultParent;
		}
	}
}
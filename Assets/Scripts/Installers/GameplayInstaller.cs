using ShootBalls.Gameplay;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.LevelPieces;
using ShootBalls.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class GameplayInstaller : MonoInstaller
	{
		[Title( "Core" )]
		[SerializeField] private PlayerInstaller _playerPrefab;
		[SerializeField] private BallInstaller _ballPrefab;
		[SerializeField] private BrickInstaller _brickPrefab;

		[Title( "FX" )]
		[SerializeField] private ScreenColorShifter.Settings _screenColor;
		[ListDrawerSettings( ListElementLabelName = "GetDisplayLabel" ), HideReferenceObjectPicker]
		[SerializeReference] private IGlobalFxProcessor.ISettings[] _globalFxSettings;

		public override void InstallBindings()
		{
			Container.Bind<GameModel>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<GameController>()
				.AsSingle();

			Container.Bind<BrickList>()
				.AsSingle();

			Container.BindFactory<PlayerController, PlayerController.Factory>()
				.FromSubContainerResolve()
				.ByNewContextPrefab( _playerPrefab )
				.WithGameObjectName( _playerPrefab.name )
				.UnderTransform( context => null );

			Container.BindFactory<Ball, Ball.Factory>()
				.FromSubContainerResolve()
				.ByNewContextPrefab( _ballPrefab )
				.WithGameObjectName( _ballPrefab.name )
				.UnderTransform( context => null );

			Container.BindFactory<Brick, Brick.Factory>()
				.FromPoolableMemoryPool( pool => pool
					.FromSubContainerResolve()
					.ByNewContextPrefab( _brickPrefab )
					.WithGameObjectName( _brickPrefab.name )
					.UnderTransform( context => context.Container.ResolveId<Transform>( "Pool_Bricks" ) )
				);

			BindFx();
		}

		private void BindFx()
		{
			Container.Bind<FxFactoryBus>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<ScreenBlinkController>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<ScreenColorShifter>()
				.AsSingle()
				.WithArguments( _screenColor );

			Container.BindInterfacesAndSelfTo<TimeScaleFxQueue>()
				.AsSingle();

			/* --- */

			Container.BindInterfacesAndSelfTo<GlobalFxValue>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<GlobalFxProcessorController>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<GlobalFxProcessorController>().
						AsSingle();

					foreach ( var globalFx in _globalFxSettings )
					{
						subContainer.Inject( globalFx );
						globalFx.InstallBindings();
					}
				} )
				.AsSingle();
		}
	}
}

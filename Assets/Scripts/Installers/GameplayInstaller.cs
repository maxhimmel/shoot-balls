using ShootBalls.Gameplay;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class GameplayInstaller : MonoInstaller
    {
		[SerializeField] private PlayerInstaller _playerPrefab;
		[SerializeField] private BallInstaller _ballPrefab;

		[Space]
		[SerializeField] private ScreenColorShifter.Settings _screenColor;

		public override void InstallBindings()
		{
			Container.Bind<GameModel>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<GameController>()
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

			Container.Bind<FxFactoryBus>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<ScreenBlinkController>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<ScreenColorShifter>()
				.AsSingle()
				.WithArguments( _screenColor );
		}
	}
}

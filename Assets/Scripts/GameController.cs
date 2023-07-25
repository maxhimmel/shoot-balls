using ShootBalls.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class GameController : IInitializable
	{
		private readonly GameModel _gameModel;
		private readonly PlayerController.Factory _playerFactory;
		private readonly Ball.Factory _ballFactory;

		public GameController( GameModel gameModel,
			PlayerController.Factory playerFactory,
			Ball.Factory ballFactory )
		{
			_gameModel = gameModel;
			_playerFactory = playerFactory;
			_ballFactory = ballFactory;
		}

		public void Initialize()
		{
			_gameModel.Player = _playerFactory.Create();
			_gameModel.Ball = _ballFactory.Create();

			_gameModel.Player.Died += OnPlayerDied;
		}

		private void OnPlayerDied()
		{
			Debug.Log( "<color=red>Gameover</color>" );
		}
	}
}

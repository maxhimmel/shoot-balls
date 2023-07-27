using ShootBalls.Gameplay.Player;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class GameController : IInitializable
	{
		private readonly GameModel _gameModel;
		private readonly PlayerController.Factory _playerFactory;
		private readonly Ball.Factory _ballFactory;
		private readonly Rewired.Player _playerInput;

		public GameController( GameModel gameModel,
			PlayerController.Factory playerFactory,
			Ball.Factory ballFactory,
			Rewired.Player playerInput )
		{
			_gameModel = gameModel;
			_playerFactory = playerFactory;
			_ballFactory = ballFactory;
			_playerInput = playerInput;
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
			_playerInput.AddButtonPressedDelegate( OnRespawnRequested, ReConsts.Action.Confirm );
		}

		private void OnRespawnRequested( Rewired.InputActionEventData data )
		{
			_playerInput.RemoveInputEventDelegate( OnRespawnRequested );

			_gameModel.Player.Respawn();
		}
	}
}

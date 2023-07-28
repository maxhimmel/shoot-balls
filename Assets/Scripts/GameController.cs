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
		private readonly Transform _ballSpawn;
		private readonly Transform _playerSpawn;

		public GameController( GameModel gameModel,
			PlayerController.Factory playerFactory,
			Ball.Factory ballFactory,
			Rewired.Player playerInput,

			[Inject( Id = "Spawn_BallStart" )] Transform ballSpawn,
			[Inject( Id = "Spawn_Player" )] Transform playerSpawn )
		{
			_gameModel = gameModel;
			_playerFactory = playerFactory;
			_ballFactory = ballFactory;
			_playerInput = playerInput;
			_ballSpawn = ballSpawn;
			_playerSpawn = playerSpawn;
		}

		public void Initialize()
		{
			_gameModel.Player = _playerFactory.Create();
			_gameModel.Player.Body.HolisticMove( _playerSpawn.position, _playerSpawn.rotation );

			_gameModel.Ball = _ballFactory.Create();
			_gameModel.Ball.Body.HolisticMove( _ballSpawn.position, _ballSpawn.rotation );

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

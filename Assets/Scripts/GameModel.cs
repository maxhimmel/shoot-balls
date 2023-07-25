using System.Collections.Generic;
using ShootBalls.Gameplay.LevelPieces;
using ShootBalls.Gameplay.Player;

namespace ShootBalls.Gameplay
{
	public class GameModel
	{
		public IReadOnlyList<Brick> ActiveBricks => _bricks.ActiveBricks;

		public PlayerController Player;
		public Ball Ball;

		private readonly BrickList _bricks;

		public GameModel( BrickList bricks )
		{
			_bricks = bricks;
		}
	}
}
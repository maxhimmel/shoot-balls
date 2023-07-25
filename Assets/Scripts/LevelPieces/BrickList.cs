using System.Collections.Generic;

namespace ShootBalls.Gameplay.LevelPieces
{
	public class BrickList
	{
		public IReadOnlyList<Brick> ActiveBricks => _activeBricks;

		private readonly List<Brick> _activeBricks = new List<Brick>();

		public void Add( Brick brick )
		{
			_activeBricks.Add( brick );
		}

		public void Remove( Brick brick )
		{
			_activeBricks.Remove( brick );
		}
	}
}
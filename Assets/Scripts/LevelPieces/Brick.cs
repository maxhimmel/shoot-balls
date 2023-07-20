using ShootBalls.Gameplay.Pawn;
using UnityEngine;

namespace ShootBalls.Gameplay.LevelPieces
{
	public class Brick : IPawn
    {
		public Rigidbody2D Body => _body;

		private readonly Rigidbody2D _body;

		public Brick( Rigidbody2D body )
		{
			_body = body;
		}
    }
}

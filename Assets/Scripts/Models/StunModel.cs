namespace ShootBalls.Gameplay.Pawn
{
	public class StunModel
	{
		public event System.Action<float> StunPointsChanged;
		public event System.Action<float> MaxStunPointsChanged;

		public float StunPoints { get; private set; }
		public float MaxStunPoints { get; private set; }

		public void SetStunPoints( float points )
		{
			StunPoints = points;
			StunPointsChanged?.Invoke( points );
		}

		public void SetMaxStunPoints( float points )
		{
			MaxStunPoints = points;
			MaxStunPointsChanged?.Invoke( points );
		}
	}
}
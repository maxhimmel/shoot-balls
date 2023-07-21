namespace ShootBalls.Gameplay.Pawn
{
	public interface IStunnable
	{
		bool IsStunned();

		void OnStunHit( float damage );
		void OnDirectHit( float damage );
	}
}
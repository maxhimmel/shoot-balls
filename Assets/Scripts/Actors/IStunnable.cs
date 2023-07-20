namespace ShootBalls.Gameplay.Pawn
{
	public interface IStunnable
	{
		bool IsStunned();

		/// <returns>True if the <see cref="IStunnable"/> became stunned on this hit.</returns>
		bool Hit();
	}
}
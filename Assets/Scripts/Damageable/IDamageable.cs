namespace ShootBalls.Gameplay.Pawn
{
	public interface IDamageable
	{
		/// <returns>True if damage was applied.</returns>
		bool TakeDamage( IDamageData data );
	}
}
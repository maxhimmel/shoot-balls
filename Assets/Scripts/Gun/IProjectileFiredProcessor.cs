namespace ShootBalls.Gameplay.Weapons
{
	public interface IProjectileFiredProcessor
	{
		void Notify( Projectile firedProjectile );
	}
}
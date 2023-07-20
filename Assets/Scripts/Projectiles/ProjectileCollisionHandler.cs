using System;
using ShootBalls.Gameplay.Pawn;

namespace ShootBalls.Gameplay.Weapons
{
	public abstract class ProjectileCollisionHandler : DamageHandler<Projectile, IDamageData>,
		IDisposable
	{
		public abstract void Dispose();
	}

	public interface IProjectileDamageData
	{
		System.Type HandlerType { get; }
	}

	public abstract class ProjectileDamageData<THandler> : IProjectileDamageData
		where THandler : ProjectileCollisionHandler
	{
		public virtual System.Type HandlerType => typeof( THandler );
	}
}
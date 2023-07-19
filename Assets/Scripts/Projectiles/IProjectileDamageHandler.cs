using System;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	// TODO: This is gross and should be properly thought out to be fitted into the interface thoughtfully.
	public class DamageDeliveredSignal
	{
		public Rigidbody2D HitBody;
		public Vector2 HitDirection;
	}

	public interface IProjectileDamageHandler : IDisposable
	{
		void Handle( Projectile projectile, DamageDeliveredSignal signal );

		public interface ISettings
		{
			System.Type HandlerType { get; }
		}

		public class Settings<THandler> : ISettings
			where THandler : IProjectileDamageHandler
		{
			public Type HandlerType => typeof( THandler );
		}
	}
}
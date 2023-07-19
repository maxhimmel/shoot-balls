using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public class ProjectileBounceHandler : IProjectileDamageHandler
	{
		private readonly Settings _settings;
		private readonly Rigidbody2D _body;

		private int _bounceCount;

		public ProjectileBounceHandler( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
			_body = body;
		}

		public void Handle( Projectile projectile, DamageDeliveredSignal signal )
		{
			if ( !_settings.IsUnlimited && _bounceCount++ >= _settings.Bounces )
			{
				projectile.Dispose();
			}
			else
			{
				float speed = _body.velocity.magnitude * _settings.Bounciness;
				_body.velocity = -signal.HitDirection * speed;
				_body.SetRotation( signal.HitDirection.ToLookRotation() );
			}
		}

		public void Dispose()
		{
			_bounceCount = 0;
		}

		[System.Serializable]
		public class Settings : IProjectileDamageHandler.Settings<ProjectileBounceHandler>
		{
			[HorizontalGroup]
			public bool IsUnlimited;

			[HorizontalGroup]
			[MinValue( 0 ), DisableIf( "IsUnlimited" )]
			public int Bounces;

			[Range( 0, 1 )]
			public float Bounciness = 1;
		}
	}
}
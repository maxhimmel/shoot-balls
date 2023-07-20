using ShootBalls.Gameplay.Pawn;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public class ProjectileBounceHandler : ProjectileCollisionHandler
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

		protected override bool Handle( Projectile owner, IDamageData data )
		{
			if ( !_settings.IsUnlimited && _bounceCount++ >= _settings.Bounces )
			{
				owner.Dispose();
			}
			else
			{
				float speed = _body.velocity.magnitude * _settings.Bounciness;
				_body.velocity = -data.HitNormal * speed;
				_body.SetRotation( data.HitNormal.ToLookRotation() );
			}

			return true;
		}

		public override void Dispose()
		{
			_bounceCount = 0;
		}

		[System.Serializable]
		public class Settings : ProjectileDamageData<ProjectileBounceHandler>
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
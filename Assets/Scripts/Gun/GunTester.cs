using ShootBalls.Gameplay;
using ShootBalls.Gameplay.Weapons;
using ShootBalls.Installers;
using UnityEngine;
using Zenject;

namespace ShootBalls.Test
{
	public class GunTester : IInitializable,
		IPushable
	{
		public Gun Gun => _gun;
		public Rigidbody2D Body => _body;

		private readonly Settings _settings;
		private readonly Gun.Factory _gunFactory;
		private readonly Rigidbody2D _body;

		private Gun _gun;

		public GunTester( Settings settings,
			Gun.Factory gunFactory,
			Rigidbody2D body )
		{
			_settings = settings;
			_gunFactory = gunFactory;
			_body = body;
		}

		public void Initialize()
		{
			AttachGun( _settings.GunPrefab );

			if ( _settings.FireOnStart )
			{
				_gun.StartFiring();
			}
		}

		public void AttachGun( GunInstaller prefab )
		{
			if ( _gun != null )
			{
				_gun.StopFiring();
			}

			_gun = _gunFactory.Create( prefab );
			_gun.SetOwner( this );
		}

		public void Push( Vector2 velocity )
		{
			// ...
		}

		[System.Serializable]
		public class Settings
		{
			public GunInstaller GunPrefab;

			public bool FireOnStart;
		}
	}
}

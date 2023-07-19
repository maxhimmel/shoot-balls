using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class Projectile : IOrientation,
		IPoolable<Projectile.Settings, IMemoryPool>,
		IDisposable
	{
		public event System.Action<Projectile> Disposed;

		public Vector2 Position {
			get => _body.position;
			set => _body.position = value;
		}
		public Quaternion Rotation {
			get => _body.rotation.To2DRotation();
			set => _body.SetRotation( value );
		}
		public Transform Parent {
			get => _settings.Owner;
			set => _settings.Owner = value;
		}

		private readonly Rigidbody2D _body;
		private readonly CancellationToken _onDestroyedCancelToken;

		private Settings _settings;
		private IMemoryPool _pool;

		public Projectile( Rigidbody2D body )
		{
			_body = body;

			_onDestroyedCancelToken = body.GetCancellationTokenOnDestroy();
		}

		public void OnSpawned( Settings settings, IMemoryPool pool )
		{
			_settings = settings;
			_pool = pool;

			_body.gameObject.SetActive( true );
		}

		public void Launch( Vector2 direction )
		{
			_body.AddForce( direction, ForceMode2D.Impulse );

			HandleLifetime().Forget();
		}

		private async UniTaskVoid HandleLifetime()
		{
			await TaskHelpers.DelaySeconds( _settings.Lifetime, _onDestroyedCancelToken );
			Dispose();
		}

		public void Dispose()
		{
			_pool.Despawn( this );
		}

		public void OnDespawned()
		{
			_pool = null;
			Disposed?.Invoke( this );

			_body.gameObject.SetActive( false );
		}

		public class Factory : PlaceholderFactory<Settings, Projectile> { }

		[System.Serializable]
		public class Settings
		{
			public Transform Owner { get; set; }

			public float Lifetime;
		}
	}
}

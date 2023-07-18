using System.Threading;
using Cysharp.Threading.Tasks;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class Projectile : IOrientation
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
		private readonly Settings _settings;
		private readonly CancellationToken _onDestroyedCancelToken;

		public Projectile( Rigidbody2D body,
			Settings settings )
		{
			_body = body;
			_settings = settings;

			_onDestroyedCancelToken = body.GetCancellationTokenOnDestroy();
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

		private void Dispose()
		{
			Disposed?.Invoke( this );
			GameObject.Destroy( _body.gameObject );
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

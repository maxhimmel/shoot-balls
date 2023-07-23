using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public abstract class PoolableFx : MonoBehaviour,
		IPoolable<IFxSignal, IMemoryPool>
	{
		protected Settings _settings;

		private IMemoryPool _pool;
		private bool _hasPlayed;

		[Inject]
		public void Construct( Settings settings )
		{
			_settings = settings;
		}

		public virtual void OnSpawned( IFxSignal signal, IMemoryPool pool )
		{
			_pool = pool;

			if ( _settings.AttachToSource )
			{
				transform.SetParent( signal.Parent, worldPositionStays: true );
			}
			transform.SetPositionAndRotation( signal.Position, signal.Direction.ToLookRotation() );

			Play( signal );

			_hasPlayed = true;
		}

		protected abstract void Play( IFxSignal signal );

		private void Update()
		{
			if ( _hasPlayed )
			{
				if ( !Tick() )
				{
					Dispose();
				}
			}
		}


		/// <returns>True while the FX are playing.</returns>
		protected abstract bool Tick();

		protected virtual void Dispose()
		{
			_pool.Despawn( this );
		}

		public virtual void OnDespawned()
		{
			_pool = null;
			_hasPlayed = false;
		}

		[System.Serializable]
		public class Settings
		{
			public bool AttachToSource;
		}
	}
}
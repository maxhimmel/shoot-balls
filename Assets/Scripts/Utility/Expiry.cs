using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ShootBalls.Utility
{
	public class Expiry
	{
		public float Countdown { get; private set; }

		private readonly IDisposable _disposable;

		public Expiry( float lifetime, 
			IDisposable disposable )
		{
			SetLifetime( lifetime );
			_disposable = disposable;
		}

		public void SetLifetime( float lifetime )
		{
			Countdown = lifetime;
		}

		public async UniTask Tick( CancellationToken cancelToken )
		{
			await UniTask.WaitWhile( Tick, PlayerLoopTiming.Update, cancelToken );
		}

		/// <returns>True while the lifetime has not expired.</returns>
		public bool Tick()
		{
			if ( Countdown <= 0 )
			{
				return false;
			}

			Countdown -= Time.deltaTime;
			if ( Countdown > 0 )
			{
				return true;
			}

			_disposable?.Dispose();
			return false;
		}
	}
}
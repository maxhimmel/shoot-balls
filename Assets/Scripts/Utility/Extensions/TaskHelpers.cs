using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ShootBalls.Utility
{
    public static class TaskHelpers
    {
		public delegate void ProgressUpdated( float normalizedProgress );

        public static async UniTask DelaySeconds( float seconds, CancellationToken cancellationToken = default )
		{
            if ( seconds <= 0 )
			{
				return;
			}

			if ( cancellationToken == default )
			{
				//cancellationToken = AppHelper.AppQuittingToken;
			}

			await UniTask.Delay( 
				TimeSpan.FromSeconds( seconds ), 
				DelayType.DeltaTime, 
				PlayerLoopTiming.FixedUpdate, 
				cancellationToken 
			);
		}

		public static UniTask WaitForFixedUpdate( CancellationToken cancellationToken = default )
		{
			if ( cancellationToken == default )
			{
				//cancellationToken = AppHelper.AppQuittingToken;
			}
			return UniTask.WaitForFixedUpdate( cancellationToken );
		}

        public static UniTask Cancellable( this UniTask task, CancellationToken token, bool suppressCancellationThrow = true )
		{
			var result = task.AttachExternalCancellation( token );
			if ( suppressCancellationThrow )
			{
				result = result.SuppressCancellationThrow();
			}

			return result;
		}

		public static async UniTask UpdateTimer( float duration, 
			PlayerLoopTiming loop, 
			CancellationToken cancelToken, 
			ProgressUpdated callback )
		{
			if ( duration <= 0 )
			{
				callback.Invoke( 1 );
				return;
			}

			float timer = 0;
			while ( timer < duration )
			{
				timer += Time.deltaTime;

				float progress = Mathf.Clamp01( timer / duration );
				callback.Invoke( progress );

				await UniTask.Yield( loop, cancelToken );
			}
		}
	}
}

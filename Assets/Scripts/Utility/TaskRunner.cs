using System.Threading;
using Cysharp.Threading.Tasks;

namespace ShootBalls.Utility
{
	public class TaskRunner
	{
		public bool IsRunning { get; private set; }
		public CancellationToken CancelToken => _cancelSource.Token;

		private CancellationTokenSource _cancelSource;
		private CancellationToken? _outerCancelToken;

		public TaskRunner()
		{
			_cancelSource = new CancellationTokenSource();//AppHelper.CreateLinkedCTS();
		}
		public TaskRunner( CancellationToken cancelToken )
		{
			_outerCancelToken = cancelToken;
			_cancelSource = CancellationTokenSource.CreateLinkedTokenSource( cancelToken );//AppHelper.CreateLinkedCTS( cancelToken );
		}

		public async UniTask Run( System.Func<UniTask> task )
		{
			TryCancel();

			IsRunning = true;

			await task
				.Invoke()
				.Cancellable( _cancelSource.Token );

			IsRunning = false;
		}

		public bool TryCancel()
		{
			if ( !IsRunning )
			{
				return false;
			}

			_cancelSource.Cancel();
			_cancelSource.Dispose();
			_cancelSource = _outerCancelToken.HasValue && _outerCancelToken.Value.CanBeCanceled
				? CancellationTokenSource.CreateLinkedTokenSource( _outerCancelToken.Value )//AppHelper.CreateLinkedCTS( _outerCancelToken.Value )
				: new CancellationTokenSource();//AppHelper.CreateLinkedCTS();

			return true;
		}
	}
}
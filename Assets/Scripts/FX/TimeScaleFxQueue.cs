using System.Collections.Generic;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
    public class TimeScaleFxQueue : ITickable
    {
        private readonly TimeController _timeController;
        private readonly SortedList<float, Request> _requests;

        private Request _currentRequest;
        private float _previousTimeScale = 1;

        public TimeScaleFxQueue( TimeController timeController )
        {
            _timeController = timeController;

            _requests = new SortedList<float, Request>( new ScaleComparer() );
        }

        public void AdjustForSeconds( Request request )
		{
            if ( _requests.Count <= 0 )
			{
                _previousTimeScale = Time.timeScale;
			}

            if ( _requests.TryGetValue( request.Scale, out var existingRequest ) )
			{
                if ( existingRequest.EndTime < request.EndTime )
				{
                    existingRequest.EndTime = request.EndTime;
				}
			}
            else
			{
                _requests.Add( request.Scale, request );
			}
		}

        public void Tick()
        {
            TryCycleCurrentRequest();

            if ( _currentRequest == null )
			{
                return;
			}

            if ( !_currentRequest.Tick() )
			{
                _requests.Remove( _currentRequest.Scale );
                _currentRequest = null;

                if ( _requests.Count <= 0 )
				{
                    _timeController.SetTimeScale( _previousTimeScale );
				}
			}
        }

        private void TryCycleCurrentRequest()
		{
            if ( _requests.Count <= 0 )
			{
                return;
			}

            Request priority = null;
            int priorityIndex = _requests.Values.Count - 1;

            for ( int idx = priorityIndex; idx >= 0; --idx )
			{
                var request = _requests.Values[idx];
                if ( !request.IsExpired() )
				{
                    priority = request;
                    break;
				}

                if ( request != _currentRequest )
                {
                    // We remove the current request after it ticks - not here ...
                    _requests.RemoveAt( idx );
                }
			}

			if ( priority != null && _currentRequest != priority )
			{
				_currentRequest = priority;
				_timeController.SetTimeScale( priority.Scale );
			}
		}

        public class Request
        {
            public float Scale { get; }

            public float EndTime;

            public Request( float duration, float scale )
			{
                EndTime = Time.unscaledTime + duration;
                Scale = scale;
			}

            /// <returns>True until request has expired.</returns>
            public bool Tick()
			{
                return !IsExpired();
			}

            public bool IsExpired()
			{
                return EndTime <= Time.unscaledTime;
			}
		}

		private class ScaleComparer : IComparer<float>
		{
			public int Compare( float x, float y )
			{
                return -1 * Comparer<float>.Default.Compare( x, y );
			}
		}
	}
}

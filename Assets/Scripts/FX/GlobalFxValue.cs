using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class GlobalFxValue : ILateTickable
    {
		public float Value => _velocity;
		public float AbsDelta => Mathf.Abs( Delta );
		public float Delta => _velocity - _prevVelocity;

		private readonly List<Request> _forceRequests = new List<Request>( 50 );

		private float _velocity;
		private float _prevVelocity;

		public void Add( Request request )
		{
			_forceRequests.Add( request );
		}

		public void LateTick()
		{
			_prevVelocity = _velocity;

			for ( int idx = _forceRequests.Count - 1; idx >= 0; --idx )
			{
				var request = _forceRequests[idx];

				Add( request.Force );

				if ( !request.Tick( Time.deltaTime ) )
				{
					_forceRequests.RemoveAt( idx );
				}
			}
		}

		public void Add( float value )
		{
			_velocity += value * Time.deltaTime;
		}

		public void Set( float value )
		{
			_velocity = value;
		}

		public class Request
		{
			public float Duration => Force / Deceleration;

			public float Force;
			public float Deceleration;

			/// <returns>True while <see cref="Force"/> is still being applied.</returns>
			public bool Tick( float deltaTime )
			{
				Force = Mathf.MoveTowards( Force, 0, Deceleration * deltaTime );
				return Force != 0;
			}
		}
	}
}

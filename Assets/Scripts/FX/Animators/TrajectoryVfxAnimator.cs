using System.Threading;
using Cysharp.Threading.Tasks;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Fx
{
	public class TrajectoryVfxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly FxFactoryBus _fxFactory;
		private readonly CancellationToken _onDestroyCancelToken;

		private static readonly RaycastHit2D[] _trajectoryBuffer = new RaycastHit2D[50];

		public TrajectoryVfxAnimator( Settings settings,
			FxFactoryBus fxFactory,
			CancellationToken onDestroyCancelToken )
		{
			_settings = settings;
			_fxFactory = fxFactory;
			_onDestroyCancelToken = onDestroyCancelToken;
		}

		public void Play( IFxSignal signal )
		{
			FireTrajectory( signal ).Forget();
		}

		private async UniTaskVoid FireTrajectory( IFxSignal signal )
		{
			var contactFilter = new ContactFilter2D()
			{
				useLayerMask = true,
				layerMask = _settings.Mask,
				useTriggers = _settings.QueryTriggers
			};

			float distance = _settings.TravelDistance;
			while( distance > 0 )
			{
				int hitCount = Physics2D.CircleCast(
					signal.Position, _settings.CastRadius, signal.Direction, contactFilter, _trajectoryBuffer, distance
				);

				if ( hitCount <= 0 )
				{
					_fxFactory.Create( _settings.LineFxPrefab, new FxSignal()
					{
						Position = signal.Position,
						Direction = signal.Direction * distance,
						Parent = signal.Parent
					} );

					return;
				}

				var hitResult = _trajectoryBuffer[0];

				_fxFactory.Create( _settings.LineFxPrefab, new FxSignal()
				{
					Position = signal.Position,
					Direction = signal.Direction * hitResult.distance,
					Parent = signal.Parent
				} );

				distance -= hitResult.distance;

				signal.Position = hitResult.point;
				signal.Direction = Vector2.Reflect( signal.Direction, hitResult.normal );

				if ( distance > 0 )
				{
					await TaskHelpers.DelaySeconds( _settings.SegmentStepDelay, _onDestroyCancelToken, ignoreTimeScale: true );
				}
			}
		}

		[System.Serializable]
		public class Settings : IFxAnimator.Settings<TrajectoryVfxAnimator>
		{
			public PoolableLineShape LineFxPrefab;
			public LayerMask Mask;
			[MinValue( 1 )]
			public float TravelDistance = 10;
			[MinValue( 0.1f )]
			public float CastRadius = 0.5f;
			[MinValue( 0 )]
			public float SegmentStepDelay = 0.375f;
			public bool QueryTriggers;
		}
	}
}
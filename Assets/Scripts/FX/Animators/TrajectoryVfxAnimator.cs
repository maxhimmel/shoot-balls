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

		public TrajectoryVfxAnimator( Settings settings,
			FxFactoryBus fxFactory )
		{
			_settings = settings;
			_fxFactory = fxFactory;
		}

		public void Play( IFxSignal signal )
		{
			FireTrajectory( signal ).Forget();
		}

		private async UniTaskVoid FireTrajectory( IFxSignal signal )
		{
			float distance = _settings.TravelDistance;
			while( distance > 0 )
			{
				var hitResult = Physics2D.CircleCast(
					signal.Position, _settings.CastRadius, signal.Direction, distance, _settings.Mask
				);

				if ( !hitResult.IsHit() )
				{
					_fxFactory.Create( _settings.LineFxPrefab, new FxSignal()
					{
						Position = signal.Position,
						Direction = signal.Direction * distance,
						Parent = signal.Parent
					} );

					break;
				}

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
					await TaskHelpers.DelaySeconds( _settings.SegmentStepDelay, ignoreTimeScale: true );
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
		}
	}
}
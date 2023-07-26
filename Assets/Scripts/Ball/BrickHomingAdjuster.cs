using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.LevelPieces;
using ShootBalls.Gameplay.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class BrickHomingAdjuster
	{
		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly GameModel _gameModel;
		private readonly SignalBus _signalBus;

		public BrickHomingAdjuster( Settings settings,
			Rigidbody2D body,
			GameModel gameModel,
			SignalBus signalBus )
		{
			_settings = settings;
			_body = body;
			_gameModel = gameModel;
			_signalBus = signalBus;
		}

		public void Adjust( ref IDamageData data )
		{
			if ( !TryFindBestBrick( data, out var bestBrick ) )
			{
				return;
			}

			Vector2 defaultVector = data.HitNormal;
			Vector2 influenceVector = (_body.position - bestBrick.Body.position).normalized;

			data.HitNormal = Vector3.Slerp( defaultVector, influenceVector, _settings.HomingInfluence );

			if ( _settings.UseInfluenceFx )
			{
				_signalBus.TryFireId( _settings.HomingInfluenceFxId, new FxSignal()
				{
					Position = _body.position,
					Direction = -influenceVector,
					Parent = _body.transform
				} );
			}

			if ( _settings.UseRawHitFx )
			{
				_signalBus.TryFireId( _settings.RawHitFxId, new FxSignal()
				{
					Position = _body.position,
					Direction = -defaultVector,
					Parent = _body.transform
				} );
			}
		}

		private bool TryFindBestBrick( IDamageData data, out Brick bestBrick )
		{
			bestBrick = null;

			if ( _settings.HomingInfluence <= 0 )
			{
				return false;
			}

			float bestAlignment = Mathf.Infinity;
			foreach ( var brick in _gameModel.ActiveBricks )
			{
				Vector2 selfToBrick = brick.Body.position - _body.position;
				float distance = selfToBrick.magnitude;

				float dot = Vector2.Dot( selfToBrick / distance, -data.HitNormal );
				float angle = Mathf.Acos( dot ) * 180f / Mathf.PI;

				if ( angle <= _settings.MinAlignmentAngle )
				{
					if ( angle < bestAlignment )
					{
						bestAlignment = angle;
						bestBrick = brick;
					}
				}
			}

			return bestBrick != null;
		}

		[System.Serializable]
		public class Settings
		{
			[Range( 0, 1 )]
			public float HomingInfluence;
			[Range( 0, 180 )]
			public float MinAlignmentAngle;

			[HorizontalGroup( "InfluenceFx", Width = 15 ), ToggleLeft, HideLabel]
			public bool UseInfluenceFx;
			[HorizontalGroup( "InfluenceFx" ), EnableIf( "UseInfluenceFx" )]
			public string HomingInfluenceFxId = "Launched_HomingInfluence";

			[HorizontalGroup( "RawHitFx", Width = 15 ), ToggleLeft, HideLabel]
			public bool UseRawHitFx;
			[HorizontalGroup( "RawHitFx" ), EnableIf( "UseRawHitFx" )]
			public string RawHitFxId = "Launched_RawHitDirection";
		}
	}
}
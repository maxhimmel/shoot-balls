using Cysharp.Threading.Tasks;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Movement
{
	public class DodgeController
	{
		public bool IsDodging => _isDodging;
		public Vector2 Direction => _direction;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;

		private static readonly int[] _colliderLayers = new int[10];
		private static readonly Collider2D[] _colliders = new Collider2D[10];

		private bool _isDodging;
		private Vector2 _direction;
		private float _cooldownEndTime;

		public DodgeController( Settings settings,
			Rigidbody2D body,
			SignalBus signalBus )
		{
			_settings = settings;
			_body = body;
			_signalBus = signalBus;
		}

		public void Dodge( Vector2 direction )
		{
			if ( !CanDodge() )
			{
				return;
			}

			_signalBus.FireId( "Dodge", new FxSignal()
			{
				Position = _body.position,
				Direction = direction,
				Parent = _body.transform.parent
			} );

			_cooldownEndTime = Time.timeSinceLevelLoad + _settings.Cooldown;

			UpdateDodge( direction ).Forget();
		}

		private bool CanDodge()
		{
			return !_isDodging
				&& _settings.MaxDistance != 0
				&& _cooldownEndTime < Time.timeSinceLevelLoad;
		}

		private async UniTaskVoid UpdateDodge( Vector2 direction )
		{
			var hitResult = Physics2D.CircleCast(
				_body.position, _settings.CastRadius, direction.normalized, _settings.MaxDistance, _settings.CollisionLayer
			);

			Vector2 destination = hitResult.IsHit()
				? hitResult.point
				: _body.position + direction * _settings.MaxDistance;

			if ( _settings.Speed <= 0 )
			{
				_body.position = destination;
				return;
			}

			_isDodging = true;
			_direction = direction;

			int colliderCount = _body.GetAttachedColliders( _colliders );
			int dodgeLayer = LayerMask.NameToLayer( _settings.DodgeLayerId );
			for ( int idx = 0; idx < colliderCount; ++idx )
			{
				_colliderLayers[idx] = _colliders[idx].gameObject.layer;
				_colliders[idx].gameObject.layer = dodgeLayer;
			}

			float timer = 0;
			float travelDistance = hitResult.IsHit()
				? hitResult.distance
				: _settings.MaxDistance;
			float duration = travelDistance / _settings.Speed;
			Vector2 startPos = _body.position;

			while ( timer < duration )
			{
				timer = Mathf.Min( timer + Time.fixedDeltaTime, duration );

				float progress = _settings.TravelCurve.Evaluate( timer / duration );
				Vector2 newPos = Vector2.LerpUnclamped( startPos, destination, progress );
				_body.position = newPos;

				if ( timer < duration )
				{
					await UniTask.Yield( PlayerLoopTiming.FixedUpdate );
					if ( _body == null )
					{
						return;
					}
				}
			}

			_body.velocity = direction * _settings.Speed;
			_body.position = destination;
			for ( int idx = 0; idx < colliderCount; ++idx )
			{
				_colliders[idx].gameObject.layer = _colliderLayers[idx];
			}

			_isDodging = false;
		}

		[System.Serializable]
		public class Settings
		{
			[OnInspectorGUI]
			[InfoBox( "@GetDodgeDuration()", SdfIconType.ClockHistory )]

			[MinValue( 0 ), LabelText( "Dodge Cooldown" )]
			public float Cooldown;

			[BoxGroup( "Main", ShowLabel = false )]
			[HorizontalGroup( "Main/Core" )]
			public float MaxDistance;
			[HorizontalGroup( "Main/Core" ), MinValue( 0 )]
			public float Speed;
			[BoxGroup( "Main" )]
			public AnimationCurve TravelCurve = AnimationCurve.EaseInOut( 0, 0, 1, 1 );

			[BoxGroup( "Collision", ShowLabel = false )]
			public LayerMask CollisionLayer;
			[BoxGroup( "Collision" ), MinValue( 0 )]
			public float CastRadius = 0.65f;
			[BoxGroup( "Collision" )]
			public string DodgeLayerId = "Dodge";

			private string GetDodgeDuration()
			{
				return $"Dodge duration: {MaxDistance / Speed} seconds.";
			}
		}
	}
}
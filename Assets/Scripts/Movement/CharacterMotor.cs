using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace ShootBalls.Gameplay.Movement
{
	public class CharacterMotor
	{
		private readonly Settings _settings;
		private readonly Rigidbody2D _body;

		private Vector2 _desiredVelocity;
		private Vector2 _velocity;
		private float _desiredRotationAngle;
		private float _rotationAngle;

		public CharacterMotor( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
			_body = body;
		}

		public void SetDesiredVelocity( Vector2 direction )
		{
			_desiredVelocity = direction * _settings.MaxSpeed;
		}

		public void SetDesiredRotation( Vector2 direction )
		{
			_desiredRotationAngle = Vector2.SignedAngle( Vector2.up, direction );
		}

		public void ToggleRotationUpdate( bool canUpdate )
		{
			_settings.UpdateRotation = canUpdate;
		}

		public void FixedTick()
		{
			HandleMovement();

			if ( _settings.UpdateRotation )
			{
				HandleRotation();
			}
		}

		private void HandleMovement()
		{
			_velocity = _body.velocity;

			float accelerationDelta = Time.fixedDeltaTime * _settings.Acceleration;
			_velocity = Vector2.MoveTowards( _velocity, _desiredVelocity, accelerationDelta );

			_body.velocity = _velocity;
		}

		private void HandleRotation()
		{
			_rotationAngle = _body.rotation;

			float rotationDelta = Time.fixedDeltaTime * _settings.AngleAcceleration;
			_rotationAngle = Mathf.LerpAngle( _rotationAngle, _desiredRotationAngle, rotationDelta );

			_body.MoveRotation( _rotationAngle );
		}

		[System.Serializable]
		public class Settings
		{
			[TitleGroup( "Movement" )]
			public float Acceleration;
			[TitleGroup( "Movement" )]
			public float MaxSpeed;

			[TitleGroup( "Rotation" )]
			public bool UpdateRotation = true;
			[TitleGroup( "Rotation" ), EnableIf( "UpdateRotation" )]
			public float AngleAcceleration;

#if UNITY_EDITOR
			[BoxGroup( "Info", ShowLabel = false )]

			[PropertyOrder( -1 )]
			[HorizontalGroup( "Info/Group", Width = 30 )]
			[EnumToggleButtons, HideLabel]
			[SerializeField] private Metric _metric;

			[PropertyOrder( -2 )]
			[HorizontalGroup( "Info/Group" )]
			[OnInspectorGUI]
			private void DrawReachMaxSpeedDuration()
			{
				float duration = _metric == Metric.Frames
					? GetFrames()
					: GetSeconds();

				SirenixEditorGUI.IconMessageBox( $"{duration} {_metric.ToString().ToLower()} to reach max speed.", SdfIconType.Bicycle );
			}

			private float GetFrames()
			{
				return Mathf.CeilToInt( GetSeconds() / Time.fixedDeltaTime );
			}

			private float GetSeconds()
			{
				return MaxSpeed / Acceleration;
			}

			private enum Metric
			{
				[LabelText( "s" )]
				Seconds,
				[LabelText( "f" )]
				Frames
			}
#endif
		}
	}
}

using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace ShootBalls.Movement
{
	public class CharacterMotor : IFixedTickable
	{
		private readonly Settings _settings;
		private readonly Rigidbody2D _body;

		private Vector2 _desiredVelocity;
		private Vector2 _velocity;

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

		public void FixedTick()
		{
			_velocity = _body.velocity;

			float accelerationDelta = Time.fixedDeltaTime * _settings.Acceleration;
			_velocity = Vector2.MoveTowards( _velocity, _desiredVelocity, accelerationDelta );

			_body.velocity = _velocity;
		}

		[System.Serializable]
		public class Settings
		{
			public float Acceleration;
			public float MaxSpeed;

#if UNITY_EDITOR
			[OnInspectorGUI]
			private void DrawReachMaxSpeedDuration()
			{
				float duration = MaxSpeed / Acceleration;
				int frames = Mathf.CeilToInt( duration / Time.fixedDeltaTime );
				SirenixEditorGUI.IconMessageBox( $"{duration} seconds to reach max speed.\n" +
					$"{frames} frames to reach max speed.", SdfIconType.Bicycle );
			}
#endif
		}
	}
}

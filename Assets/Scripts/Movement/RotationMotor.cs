using UnityEngine;

namespace ShootBalls.Gameplay.Movement
{
	public class RotationMotor : IRotationMotor
	{
		private readonly Settings _settings;
		private readonly Rigidbody2D _body;

		private float _desiredAngle;
		private float _angle;

		public RotationMotor( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
			_body = body;
		}

		public void SetDesiredRotation( Vector2 direction )
		{
			_desiredAngle = Vector2.SignedAngle( Vector2.up, direction );
		}

		public void FixedTick()
		{
			_angle = _body.rotation;

			float rotationDelta = _settings.AccelerationAngle * Time.deltaTime;
			_angle = Mathf.MoveTowardsAngle( _angle, _desiredAngle, rotationDelta );

			// This was the original implementation which was overlooked in error ...
			//_angle = Mathf.LerpAngle( _angle, _desiredAngle, rotationDelta ); 

			_body.MoveRotation( _angle );
		}

		[System.Serializable]
		public class Settings
		{
			public float AccelerationAngle;
		}
	}
}
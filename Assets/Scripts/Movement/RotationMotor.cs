using UnityEngine;

namespace ShootBalls.Gameplay.Movement
{
	public class RotationMotor : IRotationMotor
	{
		protected readonly Settings _settings;
		protected readonly Rigidbody2D _body;

		private float _desiredAngle;
		private float _angle;

		public RotationMotor( Settings settings,
			Rigidbody2D body )
		{
			_settings = settings;
			_body = body;
		}

		public virtual void SetDesiredRotation( Vector2 direction )
		{
			_desiredAngle = Vector2.SignedAngle( Vector2.up, direction );
		}

		public virtual void FixedTick()
		{
			_angle = _body.rotation;

			float rotationDelta = _settings.AccelerationAngle * Time.deltaTime;
			_angle = Mathf.MoveTowardsAngle( _angle, _desiredAngle, rotationDelta );

			// This was the original implementation which was overlooked in error ...
			//_angle = Mathf.LerpAngle( _angle, _desiredAngle, rotationDelta ); 

			_body.MoveRotation( GetLookRotation() );
		}

		protected virtual Quaternion GetLookRotation()
		{
			return Quaternion.Euler( 0, 0, _angle );
		}

		[System.Serializable]
		public class Settings
		{
			public float AccelerationAngle;
		}
	}
}
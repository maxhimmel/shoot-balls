using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Movement
{
	public class TiltRotationMotor : RotationMotor
	{
		private readonly Transform _renderer;

		private float _tilt;
		private Vector2 _prevVelocity;
		private Vector2 _nonZeroVelocity;

		public TiltRotationMotor( Settings settings,
			Rigidbody2D body,
			[Inject( Id = "Renderer" )] Transform renderer ) : base( settings, body )
		{
			_renderer = renderer;
		}

		public override void FixedTick()
		{
			base.FixedTick();

			var settings = _settings as Settings;

			Vector2 velocity = _body.velocity;
			Vector2 velocityDelta = velocity - _prevVelocity;

			_prevVelocity = velocity;
			if ( velocity != Vector2.zero )
			{
				_nonZeroVelocity = velocity.normalized;
			}

			float acceleration = Vector2.Dot( velocityDelta, velocity );

			float tiltAcceleration = acceleration * Time.deltaTime * settings.TiltInfluence;
			_tilt = Mathf.Clamp( _tilt + tiltAcceleration, -settings.MaxTilt, settings.MaxTilt );

			float decelerationDelta = settings.TiltDeceleration * Time.deltaTime;
			_tilt = Mathf.MoveTowards( _tilt, 0, decelerationDelta );

			Vector2 rotationAxis = _nonZeroVelocity.Rotate( -90 );
			Quaternion tiltRotation = Quaternion.AngleAxis( _tilt, rotationAxis );

			_renderer.rotation = tiltRotation * base.GetLookRotation();
		}

		[System.Serializable]
		public new class Settings : RotationMotor.Settings
		{
			public float MaxTilt;
			public float TiltInfluence;
			public float TiltDeceleration;
		}
	}
}
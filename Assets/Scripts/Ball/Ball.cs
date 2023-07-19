using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Movement;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class Ball : IPawn,
		IFixedTickable
	{
		public Rigidbody2D Body => _body;

		private readonly Rigidbody2D _body;
		private readonly CharacterMotor _motor;
		private readonly SignalBus _signalBus;
		private readonly GameModel _gameModel;

		public Ball( Rigidbody2D body,
			CharacterMotor motor,
			OnCollisionEnter2DBroadcaster collisionEnter,
			SignalBus signalBus,
			GameModel gameModel )
		{
			_body = body;
			_motor = motor;
			_signalBus = signalBus;
			_gameModel = gameModel;
			collisionEnter.Entered += OnCollisionEnter;
		}

		private void OnCollisionEnter( Collision2D collision )
		{
			if ( collision.collider.TryResolveFromBodyContext<Projectile>( out _ ) )
			{
				var contact = collision.GetContact( 0 );
				_signalBus.FireId( "Damaged", new FxSignal()
				{
					Position = contact.point,
					Direction = contact.normal,
					Parent = _body.transform
				} );
			}
		}

		public void FixedTick()
		{
			Vector2 moveDir = Vector2.zero;

			if ( _gameModel.Player != null )
			{
				moveDir = (_gameModel.Player.Body.position - _body.position).normalized;
			}

			_motor.SetDesiredVelocity( moveDir );
			_motor.FixedTick();
		}

		public class Factory : PlaceholderFactory<Ball> { }
	}
}

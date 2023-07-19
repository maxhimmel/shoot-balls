using ShootBalls.Gameplay.Fx;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay
{
	public class Ball 
    {
		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;

		public Ball( Rigidbody2D body,
			OnCollisionEnter2DBroadcaster collisionEnter,
			SignalBus signalBus )
		{
			collisionEnter.Entered += OnCollisionEnter;
			_body = body;
			_signalBus = signalBus;
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
	}
}

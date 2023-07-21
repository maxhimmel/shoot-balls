using System.Collections.Generic;
using System.Linq;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Pawn;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.LevelPieces
{
	public class Brick : IPawn,
		IDamageable
    {
		public Rigidbody2D Body => _body;

		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;
		private readonly Dictionary<System.Type, IDamageHandler> _damageHandlers;

		public Brick( Rigidbody2D body,
			IDamageHandler[] damageHandlers,
			SignalBus signalBus )
		{
			_body = body;
			_signalBus = signalBus;
			_damageHandlers = damageHandlers.ToDictionary( handler => handler.GetType() );
		}

		public bool TakeDamage( IDamageData data )
		{
			bool wasDamaged = false;

			if ( data.Causer is not Projectile )
			{
				if ( _damageHandlers.TryGetValue( data.HandlerType, out var handler ) )
				{
					wasDamaged = handler.Handle( this, data );
				}
			}

			_signalBus.FireId( wasDamaged ? "Damaged" : "Deflected", new FxSignal()
			{
				Position = data.HitPosition,
				Direction = data.HitNormal,
				Parent = _body.transform
			} );

			return false;
		}
	}
}

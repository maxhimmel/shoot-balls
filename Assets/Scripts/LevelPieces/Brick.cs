using System;
using System.Collections.Generic;
using System.Linq;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Pawn;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.LevelPieces
{
	public class Brick : IPawn,
		IDamageable,
		IStunnable,
		ITickable,
		IPoolable<IMemoryPool>,
		IDisposable
    {
		public Rigidbody2D Body => _body;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly StunController _stunController;
		private readonly SignalBus _signalBus;
		private readonly Dictionary<System.Type, IDamageHandler> _damageHandlers;

		private float _health;
		private IDamageData _recentDamage;
		private IMemoryPool _pool;

		public Brick( Settings settings,
			Rigidbody2D body,
			StunController stunController,
			IDamageHandler[] damageHandlers,
			SignalBus signalBus )
		{
			_settings = settings;
			_body = body;
			_stunController = stunController;
			_signalBus = signalBus;
			_damageHandlers = damageHandlers.ToDictionary( handler => handler.GetType() );

			_health = settings.Health;

			stunController.Stunned += OnStunned;
			stunController.Recovered += OnRecovered;
		}

		public bool TakeDamage( IDamageData data )
		{
			bool wasDamaged = false;

			if ( _damageHandlers.TryGetValue( data.HandlerType, out var handler ) )
			{
				_recentDamage = data;
				wasDamaged = handler.Handle( this, data );
			}

			_signalBus.FireId( wasDamaged ? "Damaged" : "Deflected", new FxSignal()
			{
				Position = data.HitPosition,
				Direction = -data.HitNormal,
				Parent = _body.transform
			} );

			return false;
		}

		public bool IsStunned()
		{
			return _stunController.IsStunned;
		}

		void IStunnable.OnStunHit( float damage )
		{
			_stunController.Hit( damage );
		}

		private void OnStunned()
		{
			_signalBus.FireId( "Stunned", new FxSignal()
			{
				Position = _recentDamage.HitPosition,
				Direction = -_recentDamage.HitNormal,
				Parent = _body.transform
			} );
		}

		private void OnRecovered()
		{
			_signalBus.FireId( "Recovered", new FxSignal()
			{
				Position = _body.position,
				Direction = _body.transform.up,
				Parent = _body.transform
			} );
		}

		void IStunnable.OnDirectHit( float damage )
		{
			if ( _health > 0 )
			{
				_health -= damage;

				if ( _health <= 0 )
				{
					OnDead();
				}
			}
		}

		private void OnDead()
		{
			_signalBus.FireId( "Dead", new FxSignal()
			{
				Position = _recentDamage.HitPosition,
				Direction = -_recentDamage.HitNormal,
				Parent = _body.transform
			} );
		}

		public void Tick()
		{
			_stunController.Tick();
		}

		public void OnSpawned( IMemoryPool pool )
		{
			_pool = pool;

			_body.gameObject.SetActive( true );
		}

		public void Dispose()
		{
			_pool?.Despawn( this );
		}

		public void OnDespawned()
		{
			_pool = null;

			_body.gameObject.SetActive( false );
		}

		public class Factory : PlaceholderFactory<Brick> { }

		[System.Serializable]
		public class Settings
		{
			[HideLabel]
			public StunController.Settings Stun;
			[MinValue( 0 )]
			public float Health;
		}
	}
}

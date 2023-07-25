using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Pawn;
using ShootBalls.Utility;
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
		private bool IsDead => _health <= 0;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly StunController _stunController;
		private readonly DamageHandlerController _damageController;
		private readonly SignalBus _signalBus;
		private readonly CancellationToken _onDestroyedCancelToken;

		private float _health;
		private IMemoryPool _pool;

		private static readonly Collider2D[] _explosionBuffer = new Collider2D[50];

		public Brick( Settings settings,
			Rigidbody2D body,
			StunController stunController,
			DamageHandlerController damageController,
			SignalBus signalBus )
		{
			_settings = settings;
			_body = body;
			_stunController = stunController;
			_damageController = damageController;
			_signalBus = signalBus;
			_onDestroyedCancelToken = _body.GetCancellationTokenOnDestroy();

			_health = settings.Health;
		}

		public void OnSpawned( IMemoryPool pool )
		{
			_pool = pool;

			_body.gameObject.SetActive( true );
		}

		public void Tick()
		{
			if ( IsDead )
			{
				return;
			}

			_stunController.Tick();
		}

		public bool TakeDamage( IDamageData data )
		{
			return _damageController.TakeDamage( this, data );
		}

		public bool IsStunned()
		{
			return _stunController.IsStunned;
		}

		void IStunnable.OnStunHit( float damage )
		{
			_stunController.Hit( damage );
		}

		void IStunnable.OnDirectHit( float damage )
		{
			if ( !IsDead )
			{
				_health -= damage;

				if ( IsDead )
				{
					OnDead();
				}
			}
		}

		private void OnDead()
		{
			FireDeathExplosion();

			DelayedDispose().Forget();

			_signalBus.FireId( "Dead", new FxSignal()
			{
				Position = _body.position,
				Direction = -_damageController.RecentDamage.HitNormal,
				Parent = _body.transform
			} );
		}

		private void FireDeathExplosion()
		{
			int overlapCount = Physics2D.OverlapCircleNonAlloc(
				_body.position, _settings.ExplosionRadius, _explosionBuffer, _settings.ExplosionLayer
			);

			for ( int idx = 0; idx < overlapCount; ++idx )
			{
				var explosion = _explosionBuffer[idx];
				if ( explosion.attachedRigidbody == null )
				{
					continue;
				}

				explosion.attachedRigidbody.AddExplosionForce(
					_settings.ExplosionForce,
					_body.position,
					_settings.ExplosionRadius,
					ForceMode2D.Impulse
				);
			}
		}

		private async UniTaskVoid DelayedDispose()
		{
			float timer = 0;
			while ( timer < _settings.DeathAnimDuration )
			{
				timer += Time.deltaTime;
				await UniTask.Yield( PlayerLoopTiming.Update, _onDestroyedCancelToken );
			}

			Dispose();
		}

		public void Dispose()
		{
			if ( _pool != null )
			{
				_pool.Despawn( this );
			}
			else
			{
				// TODO: Remove this 'else' - it only exists because these bricks have been manually placed in the scene.
				OnDespawned();
			}
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
			[FoldoutGroup( "Health" ), HideLabel]
			public StunController.Settings Stun;
			[FoldoutGroup( "Health" ), MinValue( 0 )]
			public float Health;
			[FoldoutGroup( "Health" ), HideLabel]
			public DamageHandlerController.Settings Damage;

			[FoldoutGroup( "Death" ), MinValue( 0 )]
			public float DeathAnimDuration = 0.375f;

			[Space]
			[FoldoutGroup( "Death" )]
			public LayerMask ExplosionLayer;
			[FoldoutGroup( "Death" )]
			public float ExplosionRadius;
			[FoldoutGroup( "Death" )]
			public float ExplosionForce;
		}
	}
}

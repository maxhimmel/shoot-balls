using ShootBalls.Gameplay.Fx;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Pawn
{
	public class StunController
	{
		public event System.Action Stunned;
		public event System.Action Recovered;

		public bool IsStunned => _stunTimer > 0;
		public bool IsDamaged => _stunPoints != _settings.StunPoints;

		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;

		private float _stunPoints;
		private float _stunTimer;

		public StunController( Settings settings,
			Rigidbody2D body,
			SignalBus signalBus )
		{
			_settings = settings;
			_body = body;
			_signalBus = signalBus;
			_stunPoints = settings.StunPoints;
		}

		public void AddStunPoints( float points )
		{
			_stunPoints = Mathf.Min( _stunPoints + points, _settings.StunPoints );
		}

		/// <returns>True if the <see cref="StunController"/> was stunned on this hit.</returns>
		public bool Hit( float damage )
		{
			if ( _stunPoints > 0 )
			{
				_stunPoints -= damage;
				if ( _stunPoints <= 0 )
				{
					OnStunned();
					return true;
				}
			}

			return false;
		}

		private void OnStunned()
		{
			_stunPoints = 0;
			_stunTimer = _settings.StunDuration;

			_signalBus.FireId( "Stunned", new FxSignal()
			{
				Position = _body.position,
				Direction = _body.transform.up,
				Parent = _body.transform
			} );

			Stunned?.Invoke();
		}

		/// <returns>True while stunned.</returns>
		public bool Tick()
		{
			_stunTimer -= Time.deltaTime;
			if ( !IsStunned )
			{
				if ( _stunPoints <= 0 )
				{
					OnRecovered();
				}
				return false;
			}

			return true;
		}

		private void OnRecovered()
		{
			_stunTimer = 0;
			_stunPoints = _settings.StunPoints;

			_signalBus.FireId( "Recovered", new FxSignal()
			{
				Position = _body.position,
				Direction = _body.transform.up,
				Parent = _body.transform
			} );

			Recovered?.Invoke();
		}

		[System.Serializable]
		public class Settings
		{
			[MinValue( 1 )]
			public float StunPoints = 3;

			[MinValue( 0 )]
			public float StunDuration = 3;
		}
	}
}
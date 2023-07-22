using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Pawn
{
	public class StunController
	{
		public event System.Action Stunned;
		public event System.Action Recovered;

		public bool IsStunned => _stunTimer > 0;
		public bool IsDamaged => _stunPoints != _settings.StunPoints;

		private readonly Settings _settings;

		private float _stunPoints;
		private float _stunTimer;

		public StunController( Settings settings )
		{
			_settings = settings;

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
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
		public bool IsDamaged => _stunModel.StunPoints != _settings.StunPoints;

		private readonly StunModel _stunModel;
		private readonly Settings _settings;
		private readonly Rigidbody2D _body;
		private readonly SignalBus _signalBus;

		private float _stunTimer;

		public StunController( StunModel stunModel,
			Settings settings,
			Rigidbody2D body,
			SignalBus signalBus )
		{
			_stunModel = stunModel;
			_settings = settings;
			_body = body;
			_signalBus = signalBus;

			stunModel.SetMaxStunPoints( settings.StunPoints );
			stunModel.SetStunPoints( settings.StunPoints );
		}

		/// <returns>True if the <see cref="StunController"/> was stunned on this hit.</returns>
		public bool Hit( float damage )
		{
			if ( _stunModel.StunPoints > 0 )
			{
				AddStunPoints( -damage );
				if ( _stunModel.StunPoints <= 0 )
				{
					OnStunned();
					return true;
				}
			}

			return false;
		}

		public void AddStunPoints( float points )
		{
			float clampedPoints = Mathf.Clamp( _stunModel.StunPoints + points, 0, _settings.StunPoints );
			_stunModel.SetStunPoints( clampedPoints );
		}

		private void OnStunned()
		{
			_stunModel.SetStunPoints( 0 );
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
				if ( _stunModel.StunPoints <= 0 )
				{
					OnRecovered();
				}
				return false;
			}

			return true;
		}

		private void OnRecovered()
		{
			Restore();

			_signalBus.FireId( "Recovered", new FxSignal()
			{
				Position = _body.position,
				Direction = _body.transform.up,
				Parent = _body.transform
			} );

			Recovered?.Invoke();
		}

		public void Restore()
		{
			_stunTimer = 0;
			_stunModel.SetStunPoints( _stunModel.MaxStunPoints );
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
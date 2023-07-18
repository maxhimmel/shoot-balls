using System;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class FxListener :
		IInitializable,
		IDisposable
	{
		private readonly SignalBus _signalBus;
		private readonly string _fxId;
		private readonly IFxAnimator[] _fxAnimators;

		public FxListener( SignalBus signalBus,
			string fxId,
			IFxAnimator[] fxAnimators )
		{
			_signalBus = signalBus;
			_fxId = fxId;
			_fxAnimators = fxAnimators;
		}

		public void Initialize()
		{
			_signalBus.SubscribeId<FxSignal>( _fxId, OnFxFired );
		}

		public void Dispose()
		{
			_signalBus.TryUnsubscribeId<FxSignal>( _fxId, OnFxFired );
		}

		private void OnFxFired( FxSignal fxSignal )
		{
			foreach ( var fx in _fxAnimators )
			{
				fx.Play( fxSignal );
			}
		}
	}
}

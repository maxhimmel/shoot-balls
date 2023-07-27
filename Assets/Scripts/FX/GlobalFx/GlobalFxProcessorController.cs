using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class GlobalFxProcessorController : ITickable
	{
		private readonly GlobalFxValue _globalFx;
		private readonly IGlobalFxProcessor[] _fxListeners;

		public GlobalFxProcessorController( GlobalFxValue globalFx,
			IGlobalFxProcessor[] fxListeners )
		{
			_globalFx = globalFx;
			_fxListeners = fxListeners;
		}

		public void Tick()
		{
			foreach ( var fx in _fxListeners )
			{
				fx.Tick( _globalFx );
			}
		}
	}
}
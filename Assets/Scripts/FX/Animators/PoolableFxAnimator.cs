using System;

namespace ShootBalls.Gameplay.Fx
{
	public class PoolableFxAnimator : IFxAnimator
	{
		private readonly Settings _settings;
		private readonly FxFactoryBus _fxFactory;

		public PoolableFxAnimator( Settings settings,
			FxFactoryBus fxFactory )
		{
			_settings = settings;
			_fxFactory = fxFactory;
		}

		public void Play( IFxSignal signal )
		{
			_fxFactory.Create( _settings.Prefab, signal );
		}

		[System.Serializable]
		public class Settings : IFxAnimator.ISettings
		{
			public Type AnimatorType => typeof( PoolableFxAnimator );

			public PoolableFx Prefab;
		}
	}
}
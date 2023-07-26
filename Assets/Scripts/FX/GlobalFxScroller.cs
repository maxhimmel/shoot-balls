using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class GlobalFxScroller : ITickable
	{
		private readonly Settings _settings;
		private readonly GlobalFxValue _globalFx;

		public GlobalFxScroller( Settings settings,
			GlobalFxValue globalFx )
		{
			_settings = settings;
			_globalFx = globalFx;
		}

		public void Tick()
		{
			_globalFx.Add( _settings.ScrollSpeed );
		}

		[System.Serializable]
		public class Settings
		{
			public float ScrollSpeed;
		}
	}
}

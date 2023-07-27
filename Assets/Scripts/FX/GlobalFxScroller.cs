namespace ShootBalls.Gameplay.Fx
{
	public class GlobalFxScroller : IGlobalFxProcessor
	{
		private readonly Settings _settings;

		public GlobalFxScroller( Settings settings )
		{
			_settings = settings;
		}

		public void Tick( GlobalFxValue globalFx )
		{
			globalFx.Add( _settings.ScrollSpeed );
		}

		[System.Serializable]
		public class Settings : IGlobalFxProcessor.Settings<GlobalFxScroller>
		{
			public float ScrollSpeed;

			protected override object GetBindingArgs()
			{
				return this;
			}
		}
	}
}

using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Weapons
{
	public class AmmoClipProcessor : AmmoProcessor<AmmoClipProcessor, AmmoClipProcessor.Settings>
	{
		public override AmmoData AmmoData => new AmmoData( _currentClipCount, _settings.ClipSize );

		private int _currentClipCount;

		public AmmoClipProcessor( Settings settings ) 
			: base( settings )
		{
			_currentClipCount = settings.ClipSize;
		}

		protected override bool HasAmmo()
		{
			return _currentClipCount > 0;
		}

		protected override int ReduceAmmo()
		{
			_currentClipCount = Mathf.Max( 0, _currentClipCount - 1 );
			return _currentClipCount;
		}

		protected override void ReplenishAmmo()
		{
			_currentClipCount = _settings.ClipSize;
		}

		[System.Serializable]
		public new class Settings : AmmoProcessor<AmmoClipProcessor, Settings>.Settings
		{
			[MinValue( 1 )]
			public int ClipSize;
		}
	}
}
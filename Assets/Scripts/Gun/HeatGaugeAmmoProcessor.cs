using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Weapons
{
	public class HeatGaugeAmmoProcessor  : AmmoProcessor<HeatGaugeAmmoProcessor, HeatGaugeAmmoProcessor.Settings>
	{
		public override AmmoData AmmoData => new AmmoData( _gauge, _settings.OverheatThreshold );

		private float _gauge;

		public HeatGaugeAmmoProcessor( Settings settings )
			: base( settings )
		{
			_gauge = settings.OverheatThreshold;
		}

		protected override int ReduceAmmo()
		{
			_gauge = Mathf.Max( 0, _gauge - _settings.HeatPerShot );
			return Mathf.CeilToInt( _gauge );
		}

		protected override void ReplenishAmmo()
		{
			_gauge = _settings.OverheatThreshold;
		}

		public override void FixedTick()
		{
			base.FixedTick();

			if ( HasAmmo() )
			{
				float coolantAcceleration = Time.deltaTime * _settings.CoolantSpeed;
				_gauge = Mathf.Min( _settings.OverheatThreshold, _gauge + coolantAcceleration );

				FireAmmoSignal();
			}
		}

		protected override bool HasAmmo()
		{
			return _gauge > 0;
		}

		[System.Serializable]
		public new class Settings : AmmoProcessor<HeatGaugeAmmoProcessor, Settings>.Settings
		{
			[MinValue( 0 )]
			public float OverheatThreshold;
			public float HeatPerShot;
			public float CoolantSpeed;
		}
	}
}
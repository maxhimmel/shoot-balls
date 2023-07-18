using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public abstract class AmmoProcessor<TProcessor, TSettings> : 
		IAmmoHandler,
		IGunTickable
		where TProcessor : AmmoProcessor<TProcessor, TSettings>
		where TSettings : AmmoProcessor<TProcessor, TSettings>.Settings
	{
		public event Action Emptied;

		public delegate void UpdateAmmo( float normalizedAmmo );
		public event UpdateAmmo AmmoUpdated;

		public delegate void UpdateReloadTimer( float normalizedDuration );
		public event UpdateReloadTimer ReloadTimerUpdated;

		public abstract AmmoData AmmoData { get; }

		protected readonly TSettings _settings;

		private bool _isReloadRequested;
		protected float _reloadEndTime;

		public AmmoProcessor( TSettings settings )
		{
			_settings = settings;
		}

		public void FireEnding()
		{
			int remainingAmmo = ReduceAmmo();

			FireAmmoSignal();

			if ( remainingAmmo <= 0 )
			{
				OnAmmoEmptied();
			}
		}

		protected void FireAmmoSignal()
		{
			AmmoUpdated?.Invoke( AmmoData.Normalized );
		}

		/// <returns>Remaining ammo count.</returns>
		protected abstract int ReduceAmmo();

		protected void OnAmmoEmptied()
		{
			Emptied?.Invoke();

			if ( _settings.AutoReload )
			{
				Reload();
			}
		}

		public virtual void Reload()
		{
			if ( !_isReloadRequested && AmmoData.Normalized < 1 )
			{
				_isReloadRequested = true;
				_reloadEndTime = Time.timeSinceLevelLoad + _settings.ReloadDuration;
			}
		}

		public virtual void FixedTick()
		{
			if ( !_isReloadRequested )
			{
				return;
			}
			else if ( IsReloading() )
			{
				FireReloadTimeSignal();
				return;
			}

			_isReloadRequested = false;
			FireReloadTimeSignal();

			ReplenishAmmo();
			FireAmmoSignal();
		}

		public bool CanFire()
		{
			return !IsReloading() && HasAmmo();
		}

		private bool IsReloading()
		{
			return _reloadEndTime > Time.timeSinceLevelLoad;
		}

		protected abstract bool HasAmmo();

		protected void FireReloadTimeSignal()
		{
			float remainingTime = _reloadEndTime - Time.timeSinceLevelLoad;

			ReloadTimerUpdated?.Invoke( Mathf.Clamp01( 1 - remainingTime / _settings.ReloadDuration ) );
		}

		protected abstract void ReplenishAmmo();

		public abstract class Settings : IGunModule
		{
			public Type ModuleType => typeof( TProcessor );

			[BoxGroup( "Reloading", ShowLabel = false )]
			
			[HorizontalGroup( "Reloading/Main" )]
			public bool AutoReload;
			
			[HorizontalGroup( "Reloading/Main" )]
			[MinValue( 0 )]
			public float ReloadDuration;
		}
	}
}
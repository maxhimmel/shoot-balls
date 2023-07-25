using System.Collections.Generic;
using System.Linq;
using ShootBalls.Gameplay.Fx;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Pawn
{
	public class DamageHandlerController
	{
		public IDamageData RecentDamage { get; private set; }

		private readonly Dictionary<System.Type, IDamageHandler> _handlers;
		private readonly Settings _settings;
		private readonly SignalBus _signalBus;

		public DamageHandlerController( Settings settings,
			IDamageHandler[] handlers,
			SignalBus signalBus )
		{
			_handlers = handlers.ToDictionary( handler => handler.GetType() );
			_settings = settings;
			_signalBus = signalBus;
		}

		/// <returns>True if damage was applied.</returns>
		public bool TakeDamage( IPawn owner, IDamageData data )
		{
			bool wasDamaged = false;

			if ( _handlers.TryGetValue( data.HandlerType, out var handler ) )
			{
				RecentDamage = data;
				wasDamaged = handler.Handle( owner, data );
			}

			if ( _settings.LogDamageResult )
			{
				Debug.Log( $"{owner.Body.name} {(wasDamaged ? "damaged!" : "deflected.")}\n" +
					$"<b>Instigator:</b> {data.Instigator.Body.name} | <b>Causer:</b> {data.Causer.Body.name}" );
			}

			if ( wasDamaged )
			{
				if ( _settings.UseDamagedFx )
				{
					_signalBus.FireId( _settings.DamagedFxId, new FxSignal()
					{
						Position = data.HitPosition,
						Direction = -data.HitNormal,
						Parent = owner.Body.transform
					} );
				}
			}
			else
			{
				if ( _settings.UseDeflectedFx )
				{
					_signalBus.FireId( _settings.DeflectedFxId, new FxSignal()
					{
						Position = data.HitPosition,
						Direction = -data.HitNormal,
						Parent = owner.Body.transform
					} );
				}
			}

			return false;
		}

		[System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "Damage Controller" ), ToggleLeft]
			public bool LogDamageResult;

			[HorizontalGroup( "Damage Controller/Damage", Width = 15 ), ToggleLeft, HideLabel]
			public bool UseDamagedFx = true;
			[HorizontalGroup( "Damage Controller/Damage" ), EnableIf( "UseDamagedFx" )]
			public string DamagedFxId = "Damaged";

			[HorizontalGroup( "Damage Controller/Deflect", Width = 15 ), ToggleLeft, HideLabel]
			public bool UseDeflectedFx = true;
			[HorizontalGroup( "Damage Controller/Deflect" ), EnableIf( "UseDeflectedFx" )]
			public string DeflectedFxId = "Deflected";
		}
	}
}
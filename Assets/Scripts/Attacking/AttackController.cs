using ShootBalls.Gameplay.Fx;
using ShootBalls.Gameplay.Pawn;
using ShootBalls.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Attacking
{
	public class AttackController
	{
		private readonly SignalBus _signalBus;

		public AttackController( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		public bool DealDamage( Request request )
		{
			bool dealtDamage = false;
			var contact = request.Collision.GetContact( 0 );

			if ( request.Collision.collider.TryResolveFromBodyContext<IDamageable>( out var damageable ) )
			{
				var damageableBody = request.Collision.collider.attachedRigidbody;
				foreach ( var layer in request.Settings.DamageLayers )
				{
					if ( damageableBody.gameObject.CanCollide( layer.HitLayer ) )
					{
						layer.Damage.Instigator = request.Instigator;
						layer.Damage.Causer = request.Causer;
						layer.Damage.HitPosition = contact.point;
						layer.Damage.HitNormal = contact.normal;

						dealtDamage = damageable.TakeDamage( layer.Damage );
						break;
					}
				}
			}

			FireFx( request, contact, dealtDamage );

			return dealtDamage;
		}

		private void FireFx( Request request, ContactPoint2D contact, bool dealtDamage )
		{
			if ( dealtDamage && request.Settings.UseSuccessFx )
			{
				_signalBus.FireId( request.Settings.SuccessFxId, new FxSignal()
				{
					Position = contact.point,
					Direction = -contact.normal,
					Parent = request.Causer.Body.transform
				} );
			}
			else if ( request.Settings.UseFailureFx )
			{
				_signalBus.FireId( request.Settings.FailureFxId, new FxSignal()
				{
					Position = contact.point,
					Direction = -contact.normal,
					Parent = request.Causer.Body.transform
				} );
			}
		}

		public class Request
		{
			public Collision2D Collision;
			public IPawn Instigator;
			public IPawn Causer;
			public Settings Settings;
		}

		[System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "@GetFxGroupName()", GroupID = "FX" )]

			[HorizontalGroup( "FX/Success", Width = 15f ), HideLabel, ToggleLeft]
			public bool UseSuccessFx = true;
			[HorizontalGroup( "FX/Success" ), LabelText( "Success" ), EnableIf( "UseSuccessFx" )]
			public string SuccessFxId = "Damaged";

			[HorizontalGroup( "FX/Failure", Width = 15f ), HideLabel, ToggleLeft]
			public bool UseFailureFx = true;
			[HorizontalGroup( "FX/Failure" ), LabelText( "Failure" ), EnableIf( "UseFailureFx" )]
			public string FailureFxId = "Deflected";

			public DamageDataByLayer[] DamageLayers;

			private string GetFxGroupName()
			{
				bool allEnabled = UseSuccessFx && UseFailureFx;
				bool allDisabled = !UseSuccessFx && !UseFailureFx;

				string details = 
					allEnabled ? "Enabled" : 
					allDisabled ? "Disabled" :
					UseSuccessFx ? "Success activated" : "Failure activated";

				return $"FX ({details})";
			}
		}
	}
}
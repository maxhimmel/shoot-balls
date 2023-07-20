using ShootBalls.Utility;
using UnityEngine;

namespace ShootBalls.Gameplay.Pawn
{
	public class KnockbackDamageHandler : DamageHandler<KnockbackDamageHandler.Settings>
	{
		protected override bool Handle( IPawn owner, Settings data )
		{
			if ( owner is IStunnable stunnable && !stunnable.IsStunned() )
			{
				return false;
			}

			owner.Body.AddForceAtPosition( -data.HitNormal * data.Knockback, data.HitPosition, ForceMode2D.Impulse );
			return true;
		}

		[System.Serializable]
		public class Settings : DamageData<KnockbackDamageHandler>
		{
			public float Knockback;
		}
	}
}
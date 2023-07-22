using UnityEngine;

namespace ShootBalls.Gameplay.Pawn
{
	public class KnockbackDamageHandler : DamageHandler<KnockbackDamageHandler.Settings>
	{
		protected override bool Handle( IPawn owner, Settings data )
		{
			owner.Body.AddForce( -data.HitNormal * data.Knockback, ForceMode2D.Impulse );
			//owner.Body.AddForceAtPosition( -data.HitNormal * data.Knockback, data.HitPosition, ForceMode2D.Impulse );
			return true;
		}

		[System.Serializable]
		public class Settings : DamageData<KnockbackDamageHandler>
		{
			public float Knockback;
		}
	}
}
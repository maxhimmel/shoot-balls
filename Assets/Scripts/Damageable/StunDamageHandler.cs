using System;
using UnityEngine.Assertions;

namespace ShootBalls.Gameplay.Pawn
{
	public class StunDamageHandler : DamageHandler<StunDamageHandler.Settings>
	{
		private readonly KnockbackDamageHandler _knockbackHandler;

		public StunDamageHandler()
		{
			_knockbackHandler = new KnockbackDamageHandler();
		}

		protected override bool Handle( IPawn owner, Settings data )
		{
			var stunnable = owner as IStunnable;
			Assert.IsNotNull( stunnable, $"The {owner} must implement {nameof( IStunnable )}." );

			bool wasHit = false;

			if ( stunnable.IsStunned() )
			{
				if ( data.DirectDamage != 0 )
				{
					wasHit = true;
					_knockbackHandler.Handle( owner, data );
					stunnable.OnDirectHit( data.DirectDamage );
				}
			}
			else
			{
				if ( data.StunDamage != 0 )
				{
					wasHit = true;
					stunnable.OnStunHit( data.StunDamage );
				}
			}

			return wasHit;
		}

		[System.Serializable]
		public class Settings : KnockbackDamageHandler.Settings
		{
			public override Type HandlerType => typeof( StunDamageHandler );

			public float StunDamage;
			public float DirectDamage;
		}
	}
}
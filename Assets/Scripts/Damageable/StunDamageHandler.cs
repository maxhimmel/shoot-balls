using System;
using ShootBalls.Utility;

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
			_knockbackHandler.Handle( owner, data );

			if ( owner is IStunnable stunnable && !stunnable.IsStunned() )
			{
				stunnable.Hit();
				return true;
			}

			return false;
		}

		[System.Serializable]
		public class Settings : KnockbackDamageHandler.Settings
		{
			public override Type HandlerType => typeof( StunDamageHandler );
		}
	}
}
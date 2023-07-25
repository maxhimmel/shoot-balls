using System;
using Sirenix.OdinInspector;
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
					TryApplyKnockback( owner, data, Settings.KnockbackMode.Direct );
					stunnable.OnDirectHit( data.DirectDamage );
				}
			}
			else
			{
				if ( data.StunDamage != 0 )
				{
					wasHit = true;
					TryApplyKnockback( owner, data, Settings.KnockbackMode.Stun );
					stunnable.OnStunHit( data.StunDamage );
				}
			}

			return wasHit;
		}

		private void TryApplyKnockback( IPawn owner, Settings data, Settings.KnockbackMode mode )
		{
			if ( ((int)mode & (int)data.ApplyKnockback) != 0 )
			{
				_knockbackHandler.Handle( owner, data );
			}
		}

		[System.Serializable]
		public class Settings : KnockbackDamageHandler.Settings
		{
			public override Type HandlerType => typeof( StunDamageHandler );

			public float StunDamage;
			public float DirectDamage;

			[EnumToggleButtons]
			public KnockbackMode ApplyKnockback = KnockbackMode.Direct;

			[Flags]
			public enum KnockbackMode
			{
				Stun	= 1 << 0,
				Direct	= 1 << 1
			}
		}
	}
}
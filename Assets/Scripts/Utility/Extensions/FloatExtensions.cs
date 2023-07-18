using UnityEngine;

namespace ShootBalls.Utility
{
    public static class FloatExtensions
    {
        public static bool DiceRoll( this float zeroToOne, bool excludeZeroes = true )
		{
            if ( excludeZeroes && zeroToOne <= 0 )
			{
                return false;
			}

            return Random.value <= zeroToOne;
		}

        public static Quaternion To2DRotation( this float angle )
		{
            return Quaternion.Euler( 0, 0, angle );
		}
    }
}

using UnityEngine;

namespace ShootBalls.Utility
{
    public static class RandomExtensions
    {
        public static int Sign()
		{
            int rand = Random.Range( 1, 3 );
            return (rand & 1) != 0
                ? -1
                : 1;
		}
    }
}

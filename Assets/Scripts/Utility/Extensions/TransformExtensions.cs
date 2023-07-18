using UnityEngine;

namespace ShootBalls.Utility
{
    public static class TransformExtensions
    {
        public static IOrientation ToData( this Transform self )
		{
            return new Orientation( self.position, self.rotation, self.parent );
		}
    }
}

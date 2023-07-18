using UnityEngine;

namespace ShootBalls.Utility
{
    public static class RectExtensions
    {
        public static Rect ToRect( this RectInt self )
        {
            return new Rect( self.x, self.y, self.width, self.height );
        }
    }
}

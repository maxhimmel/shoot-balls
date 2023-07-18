using UnityEngine;

namespace ShootBalls.Utility
{
    public static class ColorExtensions
    {
        public static Color SetAlpha( this Color self, float alpha )
		{
            self.a = alpha;
            return self;
        }

        public static Color MultAlpha( this Color self, float alpha )
        {
            self.a *= alpha;
            return self;
        }
    }
}

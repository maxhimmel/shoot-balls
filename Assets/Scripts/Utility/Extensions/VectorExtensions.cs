using UnityEngine;

namespace ShootBalls.Utility
{
    public static class VectorExtensions
    {
        public static Vector2 ToVector2( this Vector2Int self )
		{
            return new Vector2( self.x, self.y );
		}

        public static bool Approximately( this Vector3 self, Vector3 other )
		{
            if ( !Mathf.Approximately( self.x, other.x ) )
			{
                return false;
            }
            if ( !Mathf.Approximately( self.y, other.y ) )
            {
                return false;
            }
            if ( !Mathf.Approximately( self.z, other.z ) )
            {
                return false;
            }

            return true;
        }

        public static bool Approximately( this Vector2 self, Vector2 other )
        {
            if ( !Mathf.Approximately( self.x, other.x ) )
            {
                return false;
            }
            if ( !Mathf.Approximately( self.y, other.y ) )
            {
                return false;
            }

            return true;
        }

        public static float Random( this Vector2 self )
		{
            return UnityEngine.Random.Range( self.x, self.y );
        }

        public static int Random( this Vector2Int self, bool isMaxExclusive = true )
        {
            int max = isMaxExclusive
                ? self.y
                : self.y + 1;

            return UnityEngine.Random.Range( self.x, max );
        }

        public static Vector2 Rotate( this Vector2 self, float degrees )
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin( radians );
            float cos = Mathf.Cos( radians );

            float x = self.x;
            float y = self.y;
            self.x = (cos * x) - (sin * y);
            self.y = (sin * x) + (cos * y);

            return self;
        }

        public static Quaternion ToLookRotation( this Vector2 self )
		{
            return Quaternion.LookRotation( Vector3.forward, self );
        }
        public static Quaternion ToLookRotation( this Vector3 self )
        {
            return Quaternion.LookRotation( Vector3.forward, self );
        }

        public static Vector2 ToVector2( this Vector3 self )
		{
            return self;
		}

        public static Vector3Int RoundToVector3Int( this Vector3 self )
		{
            return new Vector3Int(
                Mathf.RoundToInt( self.x ),
                Mathf.RoundToInt( self.y ),
                Mathf.RoundToInt( self.z )
            );
        }

        public static Vector2Int RoundToVector2Int( this Vector2 self )
        {
            return new Vector2Int(
                Mathf.RoundToInt( self.x ),
                Mathf.RoundToInt( self.y )
            );
        }
    }
}

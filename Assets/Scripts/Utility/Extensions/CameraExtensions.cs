using UnityEngine;

namespace ShootBalls.Utility
{
    public static class CameraExtensions
    {
        public static Rect GetWorldSpaceRect( this Camera self )
		{
            var size = GetWorldSpaceSize( self );

            Vector2 position = new Vector2()
            {
                x = self.transform.position.x - size.x / 2f,
                y = self.transform.position.y - size.y / 2f
            };

            return new Rect( position, size );
        }

        public static Bounds GetWorldSpaceBounds( this Camera self )
        {
            return new Bounds( self.transform.position, self.GetWorldSpaceSize() );
        }

        public static Vector3 GetWorldSpaceSize( this Camera self )
        {
            return new Vector3( GetWorldSpaceWidth( self ), GetWorldSpaceHeight( self ), self.farClipPlane - self.nearClipPlane );
        }

        public static float GetWorldSpaceWidth( this Camera self )
        {
            return GetWorldSpaceHeight( self ) * self.aspect;
        }

        public static float GetWorldSpaceHeight( this Camera self )
		{
            return self.orthographicSize * 2f;
		}
    }
}

using UnityEngine;

namespace ShootBalls.Utility
{
    public class Orientation : IOrientation
    {
        public Vector2 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Transform Parent { get; set; }

        public Orientation( Vector2 position, Quaternion rotation, Transform parent )
        {
            Position = position;
            Rotation = rotation;
            Parent = parent;
        }

        public Orientation( Vector2 position, Quaternion rotation ) : this( position, rotation, null )
        {
        }

        public Orientation( Vector2 position ) : this( position, Quaternion.identity )
        {
        }
    }
}

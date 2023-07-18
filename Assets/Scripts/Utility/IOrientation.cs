using UnityEngine;

namespace ShootBalls.Utility
{
    public interface IOrientation
    {
        Vector2 Position { get; set; }
        Quaternion Rotation { get; set; }
        Transform Parent { get; set; }
    }
}

using System.Collections.Generic;
using ShootBalls.Utility;

namespace ShootBalls.Gameplay.Weapons
{
    public interface IFireSpread
    {
        IEnumerable<IOrientation> GetSpread( ShotSpot shotSpot );

        public interface ISettings : IGunModule { }
    }
}

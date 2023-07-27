using UnityEngine;

namespace ShootBalls.Utility
{
	public class TimeController
    {
        public float Scale => Time.timeScale;

        private readonly float _fixedDeltaTime;

        public TimeController()
		{
            _fixedDeltaTime = Time.fixedDeltaTime;
		}

        public void SetTimeScale( float scale )
		{
            Time.timeScale = scale;
            Time.fixedDeltaTime = _fixedDeltaTime * scale;
		}
    }
}

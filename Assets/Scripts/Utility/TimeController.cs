using System;
using Cysharp.Threading.Tasks;
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

        public async UniTask AdjustForSeconds( float seconds, float tempTimeScale )
		{
            if ( seconds <= 0 )
			{
                return;
			}

            float prevScale = Time.timeScale;
            SetTimeScale( tempTimeScale );

            await UniTask.Delay(
                TimeSpan.FromSeconds( seconds ),
                ignoreTimeScale: true,
                PlayerLoopTiming.Update//,
                //AppHelper.AppQuittingToken
            );

            SetTimeScale( prevScale );
		}

        public void SetTimeScale( float scale )
		{
            Time.timeScale = scale;
            Time.fixedDeltaTime = _fixedDeltaTime * scale;
		}
    }
}

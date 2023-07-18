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

        public async UniTask PauseForSeconds( float seconds )
		{
            if ( seconds <= 0 )
			{
                return;
			}

            SetTimeScale( 0 );

            await UniTask.Delay(
                TimeSpan.FromSeconds( seconds ),
                ignoreTimeScale: true,
                PlayerLoopTiming.Update//,
                //AppHelper.AppQuittingToken
            );

            SetTimeScale( 1 );
		}

        public void SetTimeScale( float scale )
		{
            Time.timeScale = scale;
            Time.fixedDeltaTime = _fixedDeltaTime * scale;
		}
    }
}

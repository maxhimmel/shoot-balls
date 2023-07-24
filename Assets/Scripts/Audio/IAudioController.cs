using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ShootBalls.Gameplay.Audio
{
	public interface IAudioController
	{
		UniTask LoadBank( string key );
		UniTask UnloadBank( string key );

		float GetVolume( string key );
		void SetVolume( string key, float volume );

		IEventInstance PlayOneShot( string key, Vector2 position );

		IEventInstance CreateInstance( string key );
	}

}
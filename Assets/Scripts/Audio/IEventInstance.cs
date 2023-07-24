using ShootBalls.Utility;

namespace ShootBalls.Gameplay.Audio
{
	public interface IEventInstance
	{
		bool IsPlaying { get; }

		void Play();
		void Stop();

		void Pause();
		void Resume();

		void SetOrientation( IOrientation orientation );
	}
}

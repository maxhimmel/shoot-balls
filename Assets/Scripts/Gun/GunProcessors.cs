using ShootBalls.Utility;

namespace ShootBalls.Gameplay.Weapons
{
	/// <summary>
	/// Invokes before fire cycle starts.
	/// </summary>
	public interface IFireStartProcessor
	{
		void FireStarting();
	}

	/// <summary>
	/// Invokes once per fire cycle.
	/// </summary>
	public interface IPreFireProcessor
	{
		void PreFire( ref IOrientation orientation );
	}

	/// <summary>
	/// Invokes after fire cycle completed.
	/// </summary>
	public interface IFireEndProcessor
	{
		void FireEnding();
	}
}
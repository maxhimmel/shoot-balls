using System;

namespace ShootBalls.Gameplay.Fx
{
	public interface IFxAnimator
	{
		void Play( IFxSignal signal );

		public interface ISettings
		{
			System.Type AnimatorType { get; }
		}

		public class Settings<TAnimator> : ISettings
			where TAnimator : IFxAnimator
		{
			public Type AnimatorType => typeof( TAnimator );
		}
	}
}
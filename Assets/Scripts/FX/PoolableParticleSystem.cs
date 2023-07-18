using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class PoolableParticleSystem : PoolableFx
	{
		private ParticleSystem _fx;

		[Inject]
		public void Construct( ParticleSystem fx )
		{
			_fx = fx;
		}

		protected override void Play( IFxSignal signal )
		{
			_fx.Play( withChildren: true );
		}

		protected override bool IsPlaying()
		{
			return _fx.IsAlive( withChildren: true );
		}
	}
}
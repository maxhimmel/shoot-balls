using ShootBalls.Gameplay.Fx;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class PoolableFxSettings : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private PoolableFx.Settings _settings;
		[SerializeField] private Mode _mode;

		public override void InstallBindings()
		{
			Container.BindInstance( _settings )
				.AsSingle();

			BindFxInstance();
		}

		private void BindFxInstance()
		{
			switch ( _mode )
			{
				case Mode.Particle:
					Container.Bind<ParticleSystem>()
						.FromMethod( GetComponentInChildren<ParticleSystem> )
						.AsSingle();
					break;
			}
		}

		private enum Mode
		{
			Particle
		}
	}
}
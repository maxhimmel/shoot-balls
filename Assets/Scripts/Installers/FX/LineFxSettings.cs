using Shapes;
using ShootBalls.Gameplay.Fx;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class LineFxSettings : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private PoolableLineShape.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind( typeof( PoolableLineShape.Settings ), typeof( PoolableFx.Settings ) )
				.FromInstance( _settings )
				.AsSingle();

			BindFxInstance();
		}

		private void BindFxInstance()
		{
			Container.Bind<Line>()
				.FromMethod( GetComponentInChildren<Line> )
				.AsSingle();
		}
	}
}
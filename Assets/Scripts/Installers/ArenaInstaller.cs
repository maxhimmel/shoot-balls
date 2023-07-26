using Shapes;
using ShootBalls.Gameplay.Fx;
using Zenject;

namespace ShootBalls.Installers
{
	public class ArenaInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<DashShapeScroller>()
				.AsSingle();

			Container.Bind<IDashable[]>()
				.FromMethod( GetComponentsInChildren<IDashable> )
				.AsSingle();
		}
	}
}

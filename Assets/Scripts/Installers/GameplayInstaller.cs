using ShootBalls.Gameplay.Fx;
using Zenject;

namespace ShootBalls.Installers
{
	public class GameplayInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<ScreenBlinkController>()
				.AsSingle();
		}
	}
}

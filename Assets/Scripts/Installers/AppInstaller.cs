using ShootBalls.Utility;
using Zenject;

namespace ShootBalls.Installers
{
	public class AppInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<InputResolver>()
				.AsSingle();

			Container.Bind<Rewired.Player>()
				.FromResolveGetter<InputResolver>( resolver => resolver.GetInput( 0 ) )
				.AsSingle();
		}
	}
}
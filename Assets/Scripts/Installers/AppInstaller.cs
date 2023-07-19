using ShootBalls.Utility;
using Zenject;

namespace ShootBalls.Installers
{
	public class AppInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			SignalBusInstaller.Install( Container );

			Container.Bind<TimeController>()
				.AsSingle();

			/* --- */

			Container.Bind<InputResolver>()
				.AsSingle();

			Container.Bind<Rewired.Player>()
				.FromResolveGetter<InputResolver>( resolver => resolver.GetInput( 0 ) )
				.AsSingle();

			/* --- */

			Container.Bind<OnCollisionEnter2DBroadcaster>()
				.FromNewComponentOnRoot()
				.AsTransient();

			/* --- */

		}
	}
}
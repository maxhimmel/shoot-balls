using ShootBalls.Gameplay.Audio;
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

			BindInput();

			/* --- */

			Container.Bind<OnCollisionEnter2DBroadcaster>()
				.FromNewComponentOnRoot()
				.AsTransient();

			/* --- */

			BindAudio();
		}

		private void BindInput()
		{
			Container.Bind<InputResolver>()
				.AsSingle();

			Container.Bind<Rewired.Player>()
				.FromResolveGetter<InputResolver>( resolver => resolver.GetInput( 0 ) )
				.AsSingle();
		}

		private void BindAudio()
		{
			Container.Bind<AudioVolumeModel>()
				.AsSingle();

			Container.Bind<IAudioController>()
				.To<AudioController>()
				.FromMethod( GetComponentInChildren<AudioController> )
				.AsSingle();
		}
	}
}
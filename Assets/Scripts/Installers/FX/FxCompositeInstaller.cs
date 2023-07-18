using Zenject;

namespace ShootBalls.Installers
{
    public class FxCompositeInstaller : MonoInstaller
    {
		public override void InstallBindings()
		{
			var fxSettings = GetComponentsInChildren<FxListenerSettings>( includeInactive: true );
			foreach ( var fx in fxSettings )
			{
				Container.Inject( fx );
				fx.InstallBindings();
			}
		}
	}
}

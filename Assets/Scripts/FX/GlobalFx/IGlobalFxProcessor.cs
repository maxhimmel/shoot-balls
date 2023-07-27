using Sirenix.Utilities;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public interface IGlobalFxProcessor
	{
		void Tick( GlobalFxValue globalFx );

		public interface ISettings : IInstaller
		{
			System.Type ProcessorType { get; }

			string GetDisplayLabel();
		}

		public abstract class Settings : ISettings
		{
			public bool IsEnabled => true;
			public abstract System.Type ProcessorType { get; }

			[Inject]
			private DiContainer _container;

			public virtual void InstallBindings()
			{
				var binding = _container.BindInterfacesTo( ProcessorType )
					.AsCached();

				var args = GetBindingArgs();
				if ( args != null )
				{
					binding.WithArguments( args );
				}
			}

			protected virtual object GetBindingArgs()
			{
				return null;
			}

			string ISettings.GetDisplayLabel()
			{
				return ProcessorType.GetNiceName().SplitPascalCase();
			}
		}

		public class Settings<TListener> : Settings
			where TListener : IGlobalFxProcessor
		{
			public override System.Type ProcessorType => typeof( TListener );
		}
	}
}
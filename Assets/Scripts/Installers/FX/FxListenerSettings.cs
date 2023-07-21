using ShootBalls.Gameplay.Fx;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class FxListenerSettings : MonoInstaller
	{
		private bool IsFxIdValid => !string.IsNullOrEmpty( _fxId );

		[InfoBox( "Fx ID must be set.", VisibleIf = "@!IsFxIdValid", InfoMessageType = InfoMessageType.Error )]
		[SerializeField] private string _fxId;

		[ShowIf( "IsFxIdValid" )]
		[ListDrawerSettings( ShowFoldout = false )]
		[SerializeReference] private IFxAnimator.ISettings[] _fxSettings = new IFxAnimator.ISettings[0];

		public override void InstallBindings()
		{
			Container.DeclareSignal<FxSignal>()
				.WithId( _fxId )
				.OptionalSubscriber();

			Container.BindInterfacesTo<FxListener>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<FxListener>()
						.AsSingle()
						.WithArguments( _fxId );

					foreach ( var settings in _fxSettings )
					{
						subContainer.BindInterfacesTo( settings.AnimatorType )
							.AsCached()
							.WithArguments( settings );
					}
				} )
				.AsCached();
		}

		[EnableIf( "@UnityEngine.Application.isPlaying" )]
		[BoxGroup, Button( ButtonSizes.Gigantic, Style = ButtonStyle.FoldoutButton, DirtyOnClick = false, Expanded = true )]
		private void Test( Vector2 localPos, Vector2 worldDir )
		{
			var signalBus = Container.TryResolve<SignalBus>();
			if ( signalBus != null )
			{
				signalBus.FireId( _fxId, new FxSignal()
				{
					Position = transform.TransformPoint( localPos ),
					Direction = worldDir,
					Parent = transform
				} );
			}
		}
	}
}
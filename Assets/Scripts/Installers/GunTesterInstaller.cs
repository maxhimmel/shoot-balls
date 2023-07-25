using Cysharp.Threading.Tasks;
using ShootBalls.Gameplay.Weapons;
using ShootBalls.Test;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers.Test
{
	[RequireComponent( typeof( Rigidbody2D ) )]
	public class GunTesterInstaller : MonoInstaller
	{
		[BoxGroup( "Settings" ), HideLabel]
		[SerializeField] private GunTester.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GunTester>()
				.AsSingle()
				.WithArguments( _settings );

			Container.BindFactory<GunInstaller, Gun, Gun.Factory>()
				.FromFactory<Gun.CustomFactory>();

			/* --- */

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponentInChildren<Rigidbody2D> )
				.AsSingle();
		}

		[EnableIf( "@UnityEngine.Application.isPlaying" )]
		[Button]
		private async void FireOnce()
		{
			StartFiring();
			{
				await UniTask.DelayFrame( 2, PlayerLoopTiming.FixedUpdate );
			}
			StopFiring();
		}

		[EnableIf( "@UnityEngine.Application.isPlaying" )]
		[ButtonGroup]
		private void StartFiring()
		{
			Container.Resolve<GunTester>()
				.Gun
				.StartFiring();
		}

		[EnableIf( "@UnityEngine.Application.isPlaying" )]
		[ButtonGroup]
		private void StopFiring()
		{
			Container.Resolve<GunTester>()
				.Gun
				.StopFiring();
		}

		[EnableIf( "@UnityEngine.Application.isPlaying" )]
		[Button( Style = ButtonStyle.FoldoutButton, Expanded = true )]
		public void AttachNewGun( GunInstaller gunPrefab )
		{
			Container.Resolve<GunTester>()
				.AttachGun( gunPrefab );
		}
	}
}
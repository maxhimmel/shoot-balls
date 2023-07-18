using ShootBalls.Gameplay.Movement;
using ShootBalls.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class PlayerInstaller : MonoInstaller
	{
		[FoldoutGroup( "Motor" ), HideLabel]
		[SerializeField] private CharacterMotor.Settings _motor;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<PlayerController>()
				.AsSingle();

			/* --- */

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponentInChildren<Rigidbody2D> )
				.AsSingle();

			/* --- */

			Container.BindInterfacesAndSelfTo<CharacterMotor>()
				.AsSingle()
				.WithArguments( _motor );
		}
	}
}

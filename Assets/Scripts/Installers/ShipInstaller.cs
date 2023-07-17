using ShootBalls.Movement;
using ShootBalls.Player;
using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class ShipInstaller : MonoInstaller
    {
		[SerializeField] private CharacterMotor.Settings _motor;

		public override void InstallBindings()
		{
			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponentInChildren<Rigidbody2D> )
				.AsSingle();

			/* --- */

			Container.BindInterfacesAndSelfTo<PlayerController>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<CharacterMotor>()
				.AsSingle()
				.WithArguments( _motor );
		}
	}
}

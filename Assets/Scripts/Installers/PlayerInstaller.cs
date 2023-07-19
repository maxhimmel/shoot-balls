using ShootBalls.Gameplay.Cameras;
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

		[FoldoutGroup( "Camera" )]
		[SerializeField] private TargetGroupAttachment.Settings _playerTarget;
		[FoldoutGroup( "Camera" )]
		[SerializeField] private TargetGroupAttachment.Settings _aimTarget;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<PlayerController>()
				.AsSingle();

			/* --- */

			Container.Bind<Rigidbody2D>()
				.FromMethod( GetComponentInChildren<Rigidbody2D> )
				.AsSingle();

			/* --- */

			Container.Bind<CharacterMotor>()
				.AsSingle()
				.WithArguments( _motor );

			Container.BindInterfacesAndSelfTo<TargetGroupAttachment>()
				.AsCached()
				.WithArguments( _playerTarget );

			Container.BindInterfacesAndSelfTo<TargetGroupAttachment>()
				.AsCached()
				.WithArguments( _aimTarget );
		}
	}
}

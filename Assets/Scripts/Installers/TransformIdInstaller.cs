using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	public class TransformIdInstaller : MonoInstaller
    {
		[SerializeField] private string[] _ids;

		public override void InstallBindings()
		{
			foreach ( var id in _ids )
			{
				Container.Bind<Transform>()
					.WithId( id )
					.FromNewComponentOnNewGameObject()
					.WithGameObjectName( id )
					.AsCached();
			}
		}
	}
}

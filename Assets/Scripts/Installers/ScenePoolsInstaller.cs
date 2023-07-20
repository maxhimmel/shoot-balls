using UnityEngine;
using Zenject;

namespace ShootBalls.Installers
{
	[CreateAssetMenu( menuName = "Shoot Balls/Scene Pools Installer" )]
    public class ScenePoolsInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private string[] _poolIds;

		private readonly object _parentId = new object();

		public override void InstallBindings()
		{
			Container.Bind<Transform>()
				.WithId( _parentId )
				.FromNewComponentOnNewGameObject()
				.WithGameObjectName( "POOLS" )
				.AsCached();

			foreach ( var id in _poolIds )
			{
				Container.Bind<Transform>()
					.WithId( id )
					.FromNewComponentOnNewGameObject()
					.WithGameObjectName( id )
					.UnderTransform( context => context.Container.ResolveId<Transform>( _parentId ) )
					.AsCached();
			}
		}
	}
}

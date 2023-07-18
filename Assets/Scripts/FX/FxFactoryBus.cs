using System.Collections.Generic;
using ShootBalls.Utility;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class FxFactoryBus
	{
		/// <summary>
		/// This ID should match a <see cref="Transform"/> within the scene being bound using a <see cref="ZenjectBinding"/>.
		/// </summary>
		private const string _containerId = "FxPool";

		private readonly DiContainer _container;
		private readonly Dictionary<PoolableFx, IMemoryPool<IFxSignal, IMemoryPool, PoolableFx>> _pools;

		public FxFactoryBus( DiContainer container )
		{
			_container = container;

			_pools = new Dictionary<PoolableFx, IMemoryPool<IFxSignal, IMemoryPool, PoolableFx>>();
		}

		public virtual PoolableFx Create( PoolableFx prefab, IFxSignal signal )
		{
			if ( !_pools.TryGetValue( prefab, out var pool ) )
			{
				pool = CreateMemoryPool( prefab );
				_pools.Add( prefab, pool );
			}

			return pool.Spawn( signal, pool );
		}

		private IMemoryPool<IFxSignal, IMemoryPool, PoolableFx> CreateMemoryPool( PoolableFx prefab )
		{
			return _container.Instantiate<MonoPoolableMemoryPool<IFxSignal, IMemoryPool, PoolableFx>>( new object[] {
				new MemoryPoolSettings( 1, int.MaxValue, PoolExpandMethods.OneAtATime ),
				new ComponentFromPrefabFactory<PoolableFx>( _container, prefab, GetPoolContainer() )
			} );
		}

		protected Transform GetPoolContainer()
		{
			return _container.ResolveId<Transform>( _containerId );
		}
	}
}
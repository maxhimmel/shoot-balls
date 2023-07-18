using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace ShootBalls.Utility
{
    public static class ZenjectExtensions
    {
		public static InstantiateCallbackConditionCopyNonLazyBinder BindUnityFactory<TValue, TFactory>( this DiContainer self, TValue prefab )
			where TValue : Component
			where TFactory : UnityFactory<TValue>
		{
			return self.Bind<TFactory>()
				.AsCached()
				.WithArguments( prefab );
		}

		public static void TrySubscribe<TSignal>( this SignalBus self, System.Action callback )
		{
			if ( self.IsSignalDeclared<TSignal>() )
			{
				self.Subscribe<TSignal>( callback );
			}
		}
		public static void TrySubscribe<TSignal>( this SignalBus self, System.Action<TSignal> callback )
		{
			if ( self.IsSignalDeclared<TSignal>() )
			{
				self.Subscribe( callback );
			}
		}
	}

	public interface IUnityFactory<TValue>
	{
		TValue Create( Vector2 position, IEnumerable<object> extraArgs = null );
		TValue Create( Vector2 position, Quaternion rotation, IEnumerable<object> extraArgs = null );
		TValue Create( Vector2 position, Quaternion rotation, Transform parent, IEnumerable<object> extraArgs = null );
		TValue Create( IOrientation placement, IEnumerable<object> extraArgs = null );
	}

	public class UnityFactory<TValue> : IUnityFactory<TValue>
		where TValue : Component
	{
		public TValue Prefab => _prefab;

		private readonly DiContainer _container;
		private readonly TValue _prefab;

		public UnityFactory( DiContainer container,
			TValue prefab )
		{
			_container = container;
			_prefab = prefab;
		}

		public TValue Create( Vector2 position, IEnumerable<object> extraArgs = null )
		{
			return Create( position, Quaternion.identity, extraArgs );
		}

		public TValue Create( Vector2 position, Quaternion rotation, IEnumerable<object> extraArgs = null )
		{
			return Create( position, rotation, null, extraArgs );
		}

		public TValue Create( Vector2 position, Quaternion rotation, Transform parent, IEnumerable<object> extraArgs = null )
		{
			if ( extraArgs == null )
			{
				extraArgs = Enumerable.Empty<object>();
			}

			var result = _container.InstantiatePrefabForComponent<TValue>( _prefab, position, rotation, parent, extraArgs );
			result.name = _prefab.name;

			if ( result.transform.parent != null && parent == null )
			{
				result.transform.SetParent( null );
			}

			return result;
		}

		public TValue Create( IOrientation placement, IEnumerable<object> extraArgs = null )
		{
			return Create( placement.Position, placement.Rotation, placement.Parent, extraArgs );
		}
	}

	public class UnityPrefabFactory<TValue> : IFactory<Object, IOrientation, IEnumerable<object>, TValue>
		where TValue : MonoBehaviour
	{
		private readonly DiContainer _container;

		public UnityPrefabFactory( DiContainer container )
		{
			_container = container;
		}

		public virtual TValue Create( Object prefab, IOrientation placement, IEnumerable<object> extraArgs = null )
		{
			if ( extraArgs == null )
			{
				extraArgs = Enumerable.Empty<object>();
			}

			var result = _container.InstantiatePrefabForComponent<TValue>( 
				prefab, 
				placement.Position, 
				placement.Rotation, 
				placement.Parent,
				extraArgs
			);
			result.name = prefab.name;

			if ( result.transform.parent != null && placement.Parent == null )
			{
				result.transform.SetParent( null );
			}

			return result;
		}
	}

	public class UnityPrefabFactory : IFactory<GameObject, IOrientation, GameObject>
	{
		[Inject]
		private readonly DiContainer _container;

		public GameObject Create( GameObject prefab, IOrientation placement )
		{
			var result = _container.InstantiatePrefab(
				prefab,
				placement.Position,
				placement.Rotation,
				placement.Parent
			);
			result.name = prefab.name;

			if ( result.transform.parent != null && placement.Parent == null )
			{
				result.transform.SetParent( null );
			}

			return result;
		}
	}

	public class ComponentFromPrefabFactory<TValue> : IFactory<TValue>
		where TValue : Component
	{
		private readonly DiContainer _container;
		private readonly TValue _prefab;
		private readonly Transform _parent;

		public ComponentFromPrefabFactory( DiContainer container,
			TValue prefab,
			[InjectOptional] Transform parent = null )
		{
			_container = container;
			_prefab = prefab;
			_parent = parent;
		}

		public TValue Create()
		{
			var result = _container.InstantiatePrefabForComponent<TValue>( _prefab, _parent );
			result.name = _prefab.name;

			return result;
		}
	}
}

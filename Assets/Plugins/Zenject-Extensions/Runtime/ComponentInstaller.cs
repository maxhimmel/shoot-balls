using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;

namespace ZenjectExtensions
{
	public class ComponentInstaller : MonoInstaller
    {
        [SerializeField] private Component[] _components = null;
        [SerializeField] private string _identifier = string.Empty;

#pragma warning disable CS0618 // Type or member is obsolete
        [SerializeField] private ZenjectBinding.BindTypes _bindType = ZenjectBinding.BindTypes.Self;
#pragma warning restore CS0618 // Type or member is obsolete

        public override void InstallBindings()
        {
            string identifier = null;
            if ( _identifier.Trim().Length > 0 )
            {
                identifier = _identifier;
            }

            foreach ( var component in _components )
            {
                if ( component == null )
                {
                    Log.Warn( $"Found null component in ComponentInstaller on object '{name}'" );
                    continue;
                }

                var componentType = component.GetType();

#pragma warning disable CS0618 // Type or member is obsolete
                switch ( _bindType )
                {
                    case ZenjectBinding.BindTypes.Self:
                    {
                        Container.Bind( componentType )
                            .WithId( identifier )
                            .FromInstance( component );
                        break;
                    }
                    case ZenjectBinding.BindTypes.BaseType:
                    {
                        Container.Bind( componentType.BaseType() )
                            .WithId( identifier )
                            .FromInstance( component );
                        break;
                    }
                    case ZenjectBinding.BindTypes.AllInterfaces:
                    {
                        Container.Bind( componentType.Interfaces() )
                            .WithId( identifier )
                            .FromInstance( component );
                        break;
                    }
                    case ZenjectBinding.BindTypes.AllInterfacesAndSelf:
                    {
                        Container.Bind( componentType.Interfaces().Concat( new[] { componentType } ).ToArray() )
                            .WithId( identifier )
                            .FromInstance( component );
                        break;
                    }
                    default:
                    {
                        throw Assert.CreateException();
                    }
                }
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }
    }
}
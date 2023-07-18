using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ShootBalls.Utility
{
	public static class SystemExtensions
	{
		public static IEnumerable<System.Type> GetSubClasses( this System.Type self )
		{
			return Assembly
				.GetAssembly( self )
				.GetTypes()
				.Where( type => !type.IsAbstract && type.IsSubclassOf( self ) );
		}

		public static IEnumerable<System.Type> GetImplementors( this System.Type self )
		{
			return Assembly
				.GetAssembly( self )
				.GetTypes()
				.Where( type => !type.IsAbstract && self.IsAssignableFrom( type ) );
		}
	}
}
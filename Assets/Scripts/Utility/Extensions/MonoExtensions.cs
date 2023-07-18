using UnityEngine;

namespace ShootBalls.Utility
{
	public static class MonoExtensions
	{
		/// <summary>
		/// Calls <see cref="Component.GetComponent{T}"/> on the <see cref="Collider2D.attachedRigidbody"/>.
		/// </summary>
		/// <returns>False if <see cref="Collider2D.attachedRigidbody"/> is null or <paramref name="component"/> wasn't found.</returns>
		public static bool TryGetComponentFromBody<TComponent>( this Collider2D collider, out TComponent component )
			where TComponent : class
		{
			var body = collider.attachedRigidbody;
			if ( body == null )
			{
				component = null;
				return false;
			}

			component = body.GetComponent<TComponent>();
			return component != null;
		}
	}
}
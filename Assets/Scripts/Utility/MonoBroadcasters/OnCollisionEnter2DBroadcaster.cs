using UnityEngine;
using Zenject;

namespace ShootBalls.Utility
{
	public class OnCollisionEnter2DBroadcaster : MonoBehaviour
    {
		public event System.Action<Collision2D> Entered;

		private void OnCollisionEnter2D( Collision2D collision )
		{
			Entered?.Invoke( collision );
		}

		public class Factory : PlaceholderFactory<OnCollisionEnter2DBroadcaster> { }
	}
}

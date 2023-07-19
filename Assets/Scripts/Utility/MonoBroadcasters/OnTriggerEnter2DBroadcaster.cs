using UnityEngine;
using Zenject;

namespace ShootBalls.Utility
{
	public class OnTriggerEnter2DBroadcaster : MonoBehaviour
	{
		public event System.Action<Collider2D> Entered;

		private void OnTriggerEnter2D( Collider2D collision )
		{
			Entered?.Invoke( collision );
		}

		public class Factory : PlaceholderFactory<OnTriggerEnter2DBroadcaster> { }
	}
}
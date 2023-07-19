using UnityEngine;
using Zenject;

namespace ShootBalls.Utility
{
	public class OnTriggerExit2DBroadcaster : MonoBehaviour
	{
		public event System.Action<Collider2D> Exited;

		private void OnTriggerExit2D( Collider2D collision )
		{
			Exited?.Invoke( collision );
		}

		public class Factory : PlaceholderFactory<OnTriggerEnter2DBroadcaster> { }
	}
}
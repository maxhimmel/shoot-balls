using System.Collections.Generic;
using UnityEngine;

namespace ShootBalls.Utility
{
	public static class AlgorithmExtensions
	{
		public static void FisherYatesShuffle<T>( this IList<T> array )
		{
			for ( int idx = array.Count - 1; idx > 0; --idx )
			{
				int randIdx = Random.Range( 0, idx + 1 );

				// Swap ...
				T temp = array[idx];
				array[idx] = array[randIdx];
				array[randIdx] = temp;
			}
		}
	}
}
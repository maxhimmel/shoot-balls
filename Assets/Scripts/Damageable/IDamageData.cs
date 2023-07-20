using ShootBalls.Utility;
using UnityEngine;

namespace ShootBalls.Gameplay.Pawn
{
	public interface IDamageData
	{
		System.Type HandlerType { get; }

		IPawn Instigator { get; set; }
		IPawn Causer { get; set; }

		Vector2 HitPosition { get; set; }
		Vector2 HitNormal { get; set; }
	}

	public class DamageData<THandler> : IDamageData
		where THandler : IDamageHandler
	{
		public virtual System.Type HandlerType => typeof( THandler );

		public IPawn Instigator { get; set; }
		public IPawn Causer { get; set; }

		public Vector2 HitPosition { get; set; }
		public Vector2 HitNormal { get; set; }
	}
}
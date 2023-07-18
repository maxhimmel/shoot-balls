using System;
using UnityEngine;

namespace ShootBalls.Gameplay.Weapons
{
	public class KnockbackProcessor : IFireEndProcessor
	{
		private readonly Settings _settings;
		private readonly IPushable _pushable;

		public KnockbackProcessor( Settings settings,
			IPushable pushable )
		{
			_settings = settings;
			_pushable = pushable;
		}

		public void FireEnding()
		{
			var pushableFacingDir = _pushable.Orientation.Rotation * Vector2.up;
			_pushable.Push( -pushableFacingDir * _settings.Impulse );
		}

		[System.Serializable]
		public class Settings : IGunModule
		{
			public Type ModuleType => typeof( KnockbackProcessor );

			public float Impulse;
		}
	}
}
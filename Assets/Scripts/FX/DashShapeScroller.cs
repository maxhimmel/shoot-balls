using Shapes;
using Zenject;

namespace ShootBalls.Gameplay.Fx
{
	public class DashShapeScroller : ITickable
	{
		private readonly GlobalFxValue _globalFx;
		private readonly IDashable[] _dashables;

		public DashShapeScroller( GlobalFxValue globalFx,
			IDashable[] dashable )
		{
			_globalFx = globalFx;
			_dashables = dashable;
		}

		public void Tick()
		{
			foreach ( var dashable in _dashables )
			{
				dashable.DashOffset = _globalFx.Value;
			}
		}
	}
}

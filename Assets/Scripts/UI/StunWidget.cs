using Shapes;
using ShootBalls.Gameplay.Pawn;
using UnityEngine;
using Zenject;

namespace ShootBalls.Gameplay.UI
{
	public class StunWidget : MonoBehaviour
    {
        [SerializeField] private Disc _points;
        [SerializeField] private Disc _fill;

		[Inject]
		private StunModel _model;

		private void OnDisable()
		{
			_model.StunPointsChanged -= OnStunPointsChanged;
			_model.MaxStunPointsChanged -= OnMaxStunPointsChanged;
		}

		private void OnEnable()
		{
			_model.StunPointsChanged += OnStunPointsChanged;
			_model.MaxStunPointsChanged += OnMaxStunPointsChanged;

			SetStunPoints( _model.StunPoints, _model.MaxStunPoints );
		}

		private void OnStunPointsChanged( float points )
		{
			SetStunPoints( points, _model.MaxStunPoints );
		}

		private void OnMaxStunPointsChanged( float points )
		{
			SetStunPoints( _model.StunPoints, points );
		}

		public void SetStunPoints( float points, float maxPoints )
		{
			_points.DashSize = maxPoints;
			_fill.AngRadiansEnd = points / maxPoints * Mathf.PI * 2;
		}
    }
}

using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShootBalls.Gameplay.Audio
{
	[System.Serializable]
	[InlineProperty]
	public class BankEvent : IComparable<BankEvent>
	{
		[BoxGroup( GroupID = "Box" )]
		[HorizontalGroup( GroupID = "Box/Hori", Title = "Event" ), HideLabel]
		public string EventName;
		[BoxGroup( GroupID = "Box" )]
		[HorizontalGroup( GroupID = "Box/Hori", Title = "Event" ), HideLabel]
		public AudioClip Clip;

		[FoldoutGroup( "Box/Params" ), PropertyRange( 0, 1 )]
		public float Volume = 1;
		[FoldoutGroup( "Box/Params" ), MinMaxSlider( -3, 3, ShowFields = true )]
		public Vector2 PitchRange = Vector2.one;
		[ToggleGroup( "Box/Params/Is3d", "3D", CollapseOthersOnExpand = false )]
		public bool Is3d = true;
		[ToggleGroup( "Box/Params/Is3d", "3D", CollapseOthersOnExpand = false )]
		[MinMaxSlider( 1, 500, ShowFields = true )]
		public Vector2 DistanceRange = new Vector2( 1, 50 );

		public int CompareTo( BankEvent other )
		{
			if ( string.IsNullOrEmpty( EventName ) )
			{
				return 1;
			}

			return EventName.CompareTo( other.EventName );
		}
	}
}
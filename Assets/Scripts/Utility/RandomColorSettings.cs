using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace ShootBalls.Utility
{
	[System.Serializable]
	public class RandomColorSettings
	{
		[VerticalGroup( "Main/Data" )]
		[MinMaxSlider( 0, 1 )]
		public Vector2 Hue;
		[VerticalGroup( "Main/Data" )]
		[MinMaxSlider( 0, 1 )]
		public Vector2 Saturation;
		[VerticalGroup( "Main/Data" )]
		[MinMaxSlider( 0, 1 )]
		public Vector2 Value;
		[VerticalGroup( "Main/Data" )]
		[MinMaxSlider( 0, 1 )]
		public Vector2 Alpha;

		public Color Generate()
		{
			Color result = Color.HSVToRGB( Hue.Random(), Saturation.Random(), Value.Random() );
			result.a = Alpha.Random();

			return result;
		}

#if UNITY_EDITOR
		private const float _changeColorRate = 0.25f;
		private Color? _color;
		private double _nextColorChangeTime;

		[HorizontalGroup( "Main", Width = 25 )]
		[VerticalGroup( "Main/Sample" )]
		[OnInspectorGUI, PropertyOrder( -1 )]
		private void DrawColorSample()
		{
			if ( _color == null )
			{
				_color = Generate();
			}

			var rect = EditorGUILayout.GetControlRect( GUILayout.ExpandHeight( true ) );
			if ( GUI.RepeatButton( rect, string.Empty ) )
			{
				if ( _nextColorChangeTime <= EditorApplication.timeSinceStartup )
				{
					_nextColorChangeTime = EditorApplication.timeSinceStartup + _changeColorRate;
					_color = Generate();
				}
			}
			SirenixEditorGUI.DrawSolidRect( rect, _color.Value );
		}
#endif
	}
}
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace ZenjectExtensions.Editor
{
	public class ZenjectBindingWindow : OdinEditorWindow
	{
		[MenuItem( "Tools/Zenject/Query Scene Bindings" )]
		private static void OpenWindow()
		{
			GetWindow<ZenjectBindingWindow>( "Scene Binding IDs" ).Show();
		}

#if ZENJECT_SKIP_SCENEBINDINGS
		[PropertyOrder( -1 )]
		[OnInspectorGUI]
		private void DrawDisableInstructions()
		{
			SirenixEditorGUI.ErrorMessageBox( 
				"ZenjectBindings have been disabled. " +
				"To re-enable them remove the <b>ZENJECT_SKIP_SCENEBINDINGS</b> script define symbol " +
				"from ProjectSettings > Player > Other Settings." );
		}
#endif

		[HideInPlayMode]
		[LabelText( "Identifiers" )]
		[ListDrawerSettings( ShowFoldout = false, IsReadOnly = true, OnTitleBarGUI = "RefreshBindings" )]
		[SerializeField] private List<BindingElement> _bindings;

		[HideInEditorMode]
		[InfoBox( "Only viewable outside play-mode.", InfoMessageType = InfoMessageType.Error )]
		[SerializeField, DisplayAsString, HideLabel] private string _placeholder = string.Empty;

		protected override void OnDestroy()
		{
			base.OnDestroy();

			EditorApplication.playModeStateChanged -= OnEnterEditMode;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			EditorApplication.playModeStateChanged += OnEnterEditMode;

			SetupBindings();
		}

		private void OnEnterEditMode( PlayModeStateChange state )
		{
			if ( state != PlayModeStateChange.EnteredEditMode )
			{
				return;
			}

			SetupBindings();
		}

		private void RefreshBindings()
		{
			if ( SirenixEditorGUI.ToolbarButton( EditorIcons.Refresh ) )
			{
				SetupBindings();
			}
		}

		private void SetupBindings()
		{
			var lookup = new Dictionary<string, List<Component>>();

#pragma warning disable CS0618 // Type or member is obsolete
			var sceneBindings = FindObjectsOfType<ZenjectBinding>( includeInactive: true );
#pragma warning restore CS0618 // Type or member is obsolete

			foreach ( var zenjectBinding in sceneBindings )
			{
				if ( zenjectBinding.Components == null || zenjectBinding.Components.Length <= 0 )
				{
					continue;
				}

				string identifier = string.IsNullOrEmpty( zenjectBinding.Identifier )
					? "NULL"
					: zenjectBinding.Identifier;

				if ( lookup.TryGetValue( identifier, out var bindings ) )
				{
					foreach ( var component in zenjectBinding.Components )
					{
						bindings.Add( component );
					}
				}
				else
				{
					lookup.Add( identifier, new List<Component>( zenjectBinding.Components ) );
				}
			}

			_bindings = new List<BindingElement>( lookup.Count );
			foreach ( var kvp in lookup )
			{
				_bindings.Add( new BindingElement()
				{
					Identifier = kvp.Key,
					Bindings = kvp.Value
				} );
			}
			_bindings.Sort();
		}

		[System.Serializable]
		[InlineProperty]
		private class BindingElement : IComparable<BindingElement>
		{
			[HorizontalGroup( 0.3f )]
			[DisplayAsString, HideLabel]
			public string Identifier;

			[HorizontalGroup]
			[ListDrawerSettings( ShowFoldout = false, IsReadOnly = true )]
			public List<Component> Bindings;

			public int CompareTo( BindingElement other )
			{
				return Identifier.CompareTo( other.Identifier );
			}
		}
	}
}

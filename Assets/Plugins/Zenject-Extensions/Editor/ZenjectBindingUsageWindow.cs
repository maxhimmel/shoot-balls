using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Zenject;

namespace ZenjectExtensions.Editor
{
	public class ZenjectBindingUsageWindow : OdinEditorWindow
	{
		[MenuItem( "Tools/Zenject/Query Asset Bindings" )]
		private static void OpenWindow()
		{
			GetWindow<ZenjectBindingUsageWindow>( "Asset Zenject Binding Usage" ).Show();
		}

		[PropertyOrder( 0 )]
		[Button( ButtonSizes.Large, Stretch = false, Icon = SdfIconType.Search, ButtonAlignment = 0 )]
		private void SearchProjectPrefabs()
		{
			_prefabs.Clear();
			_totalBindingCount = _totalGameObjectContextCount = _totalSceneContextCount = _explicitSceneContextCount = 0;

			var guids = AssetDatabase.FindAssets( "t:prefab" );
			foreach ( var guid in guids )
			{
				var assetPath = AssetDatabase.GUIDToAssetPath( guid );
				var asset = AssetDatabase.LoadAssetAtPath<GameObject>( assetPath );

#pragma warning disable CS0618 // Type or member is obsolete
				var bindings = asset.GetComponentsInChildren<ZenjectBinding>( includeInactive: true );
#pragma warning restore CS0618 // Type or member is obsolete

				if ( bindings != null && bindings.Length > 0 )
				{
					var bindingElement = new BindingElement()
					{
						Asset = asset,
						Bindings = bindings.Select( b => new BindingData( b ) ).ToArray()
					};

					_prefabs.Add( bindingElement );

					_totalBindingCount += bindings.Length;
					foreach ( var data in bindingElement.Bindings )
					{
						switch ( data.Mode )
						{
							case BindingData.ContextMode.GameObject:
								++_totalGameObjectContextCount;
								break;
							case BindingData.ContextMode.Scene:
								++_totalSceneContextCount;
								break;
							case BindingData.ContextMode.ExplicitScene:
								++_totalSceneContextCount;
								++_explicitSceneContextCount;
								break;
						}
					}
				}
			}
		}

		private int _totalBindingCount;
		private int _totalGameObjectContextCount;
		private int _totalSceneContextCount;
		private int _explicitSceneContextCount;

		[PropertyOrder( 1 )]
		[OnInspectorGUI]
		private void DrawPrefabMetaData()
		{
			SirenixEditorGUI.MessageBox( 
				$"Total Bindings: {_totalBindingCount}\n" +
				$"GameObject Contexts: {_totalGameObjectContextCount}\n" +
				$"Scene Contexts: {_totalSceneContextCount} (Explicit: {_explicitSceneContextCount})" );
		}

		[PropertyOrder( 2 )]
		[ListDrawerSettings( IsReadOnly = true, ShowIndexLabels = true ), LabelWidth( 0f )]
		[SerializeField] private List<BindingElement> _prefabs = new List<BindingElement>();


		[PropertySpace]
		

		[PropertyOrder( 3 )]
		[Button( ButtonSizes.Large, Stretch = false, Icon = SdfIconType.Search, ButtonAlignment = 0 )]
		private void SearchScenes()
		{
			_scenes.Clear();

			var guids = AssetDatabase.FindAssets( "t:scene" );
			foreach ( var guid in guids )
			{
				var assetPath = AssetDatabase.GUIDToAssetPath( guid );
				var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>( assetPath );
				var scene = EditorSceneManager.OpenScene( assetPath, OpenSceneMode.Additive );
				var rootAssets = scene.GetRootGameObjects();
				foreach ( var gameObject in rootAssets )
				{
#pragma warning disable CS0618 // Type or member is obsolete
					var bindings = gameObject.GetComponentsInChildren<ZenjectBinding>( includeInactive: true );
#pragma warning restore CS0618 // Type or member is obsolete

					if ( bindings != null && bindings.Length > 0 )
					{
						_scenes.Add( asset );
						break;
					}
				}
				EditorSceneManager.CloseScene( scene, removeScene: true );
			}
		}

		[PropertyOrder( 4 )]
		[ListDrawerSettings( IsReadOnly = true, ShowIndexLabels = true )]
		[SerializeField] private List<SceneAsset> _scenes = new List<SceneAsset>();

		[System.Serializable]
		[InlineProperty]
		private class BindingElement : IComparable<BindingElement>
		{
			[HorizontalGroup( "Asset", Title = null )]
			[HideLabel]
			public GameObject Asset;

			[HorizontalGroup( "Asset", Title = null )]
			[ListDrawerSettings( IsReadOnly = true )]
			public BindingData[] Bindings;

			public int CompareTo( BindingElement other )
			{
				return Asset.name.CompareTo( other.Asset.name );
			}
		}

		[System.Serializable]
		private class BindingData
		{
			public ContextMode Mode { get; }

			[DisplayAsString, HideLabel]
			public string ContextType;

			[HideLabel]
#pragma warning disable CS0618 // Type or member is obsolete
			public ZenjectBinding Binding;
#pragma warning restore CS0618 // Type or member is obsolete

#pragma warning disable CS0618 // Type or member is obsolete
			public BindingData( ZenjectBinding binding )
#pragma warning restore CS0618 // Type or member is obsolete
			{
				Binding = binding;

				if ( binding.UseSceneContext )
				{
					ContextType = "Scene Context (explicit)";
					Mode = ContextMode.ExplicitScene;
				}
				else
				{
					var gameObjectContext = binding.GetComponentInParent<GameObjectContext>( includeInactive: true );

					ContextType = gameObjectContext != null
						? "GameObject Context"
						: "Scene Context";

					Mode = gameObjectContext != null
						? ContextMode.GameObject
						: ContextMode.Scene;
				}
			}

			public enum ContextMode
			{
				Scene,
				ExplicitScene,
				GameObject
			}
		}
	}
}

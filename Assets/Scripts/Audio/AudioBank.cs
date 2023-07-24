using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities.Editor;
#endif

namespace ShootBalls.Gameplay.Audio
{
    [CreateAssetMenu( menuName = "Shoot Balls/Audio/Bank" )]
    public class AudioBank : ScriptableObject
    {
        public string Category => _category;
        public IEnumerable<BankEvent> Events => _events;
        private bool IsCategoryValid => !string.IsNullOrEmpty( Category );

        [ValueDropdown( "GetMixerGroupNames" )]
        [InfoBox( "Category must match a mixer group name.", VisibleIf = "@!IsCategoryValid", InfoMessageType = InfoMessageType.Error )]
        [SerializeField] private string _category;

        [ShowIf( "IsCategoryValid" )]
        [ListDrawerSettings( OnTitleBarGUI = "DrawSortButton" )]
        [OnValueChanged( "OnBankEntryChanged", IncludeChildren = true )]
        [SerializeField] private BankEvent[] _events;

        public string ExportKey( BankEvent data )
		{
            return $"{_category}/{data.EventName}";
		}

#if UNITY_EDITOR
        private IEnumerable<string> GetMixerGroupNames()
		{
            string[] guids = AssetDatabase.FindAssets( "MasterMixer" );
            if ( guids == null || guids.Length <= 0 )
			{
                throw new System.NotImplementedException( "Create an AudioMixer named 'MasterMixer'." );
			}

            string path = AssetDatabase.GUIDToAssetPath( guids[0] );
            AudioMixer mainMixer = AssetDatabase.LoadAssetAtPath<AudioMixer>( path );

            return mainMixer.FindMatchingGroups( "Master" ).Select( group => group.name );
		}

        private void OnBankEntryChanged()
		{
            foreach ( var entry in _events )
			{
                if ( string.IsNullOrEmpty( entry.EventName ) && entry.Clip != null )
				{
                    entry.EventName = entry.Clip.name;
				}
			}
		}

        private void DrawSortButton()
        {
            if ( SirenixEditorGUI.ToolbarButton( new GUIContent( EditorIcons.TriangleUp.Active, "Sort" ) ) )
            {
                _events.Sort();
            }
        }
#endif
    }
}

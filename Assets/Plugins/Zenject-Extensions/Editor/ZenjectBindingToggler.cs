using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ZenjectExtensions.Editor
{
	public static class ZenjectBindingToggler
	{
		private const string _menuPath = "Tools/Zenject/Toggle Zenject Bindings";
		private const string _zenjectBindingDisableSymbol = "ZENJECT_SKIP_SCENEBINDINGS";

		[MenuItem( _menuPath, priority = -999 )]
		private static void ToggleZenjectBinding()
		{
			if ( QueryZenjectBindingsActive( out var defines ) )
			{
				defines += $";{_zenjectBindingDisableSymbol}";
				Debug.LogWarning( $"ZenjectBinding components have been disabled." );
			}
			else
			{
				defines = defines.Replace( $";{_zenjectBindingDisableSymbol}", string.Empty );
				Debug.LogWarning( $"ZenjectBinding components have been enabled." );
			}

			PlayerSettings.SetScriptingDefineSymbols( GetBuildTarget(), defines );
		}

		[MenuItem( _menuPath, true )]
		private static bool IsZenjectBindingsActive()
		{
			bool isBindingsActive = QueryZenjectBindingsActive( out _ );
			Menu.SetChecked( _menuPath, isBindingsActive );

			return true;
		}

		private static bool QueryZenjectBindingsActive( out string defines )
		{
			defines = PlayerSettings.GetScriptingDefineSymbols( GetBuildTarget() );
			return !defines.Contains( _zenjectBindingDisableSymbol );
		}

		private static NamedBuildTarget GetBuildTarget()
		{
			return NamedBuildTarget.FromBuildTargetGroup( EditorUserBuildSettings.selectedBuildTargetGroup );
		}
	}
}
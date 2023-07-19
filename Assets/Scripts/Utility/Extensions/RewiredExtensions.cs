using Rewired;
using UnityEngine;

namespace ShootBalls.Utility
{
	public static class RewiredExtensions
	{
		public static void AddButtonPressedDelegate( this Player input,
			System.Action<InputActionEventData> callback,
			int actionId,
			InputActionEventType eventType = InputActionEventType.ButtonJustPressed,
			UpdateLoopType updateLoop = UpdateLoopType.Update )
		{
			input.AddInputEventDelegate( callback, eventType, actionId, updateLoop );
		}

		public static void AddButtonReleasedDelegate( this Player input,
			System.Action<InputActionEventData> callback,
			int actionId,
			InputActionEventType eventType = InputActionEventType.ButtonJustReleased,
			UpdateLoopType updateLoop = UpdateLoopType.Update )
		{
			input.AddInputEventDelegate( callback, eventType, actionId, updateLoop );
		}

		public static void AddAxisDelegate( this Player input,
			System.Action<InputActionEventData> callback,
			int actionId,
			InputActionEventType eventType = InputActionEventType.AxisActiveOrJustInactive,
			UpdateLoopType updateLoop = UpdateLoopType.Update )
		{
			input.AddInputEventDelegate( callback, eventType, actionId, updateLoop );
		}

		public static void AddInputEventDelegate( this Player input, 
			System.Action<InputActionEventData> callback,
			InputActionEventType eventType,
			int actionId,
			UpdateLoopType updateLoop = UpdateLoopType.Update )
		{
			input.AddInputEventDelegate( 
				callback,
				updateLoop, 
				eventType,
				actionId 
			);
		}

		public static void EnableMapRuleSet( this Player input, string ruleSetTag, bool isEnabled )
		{
			var ruleSet = input.controllers.maps.mapEnabler.ruleSets.Find( set => set.tag == ruleSetTag );
			ruleSet.enabled = isEnabled;

			input.controllers.maps.mapEnabler.Apply();
		}

		public static Vector2 GetClampedAxis2D( this Player input, int xAxisActionId, int yAxisActionId, float maxLength = 1 )
		{
			var rawInput = input.GetAxis2D( xAxisActionId, yAxisActionId );
			return Vector2.ClampMagnitude( rawInput, maxLength );
		}
	}
}
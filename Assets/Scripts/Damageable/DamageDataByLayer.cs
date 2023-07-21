using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace ShootBalls.Gameplay.Pawn
{
	[System.Serializable]
	public class DamageDataByLayer
	{
		public LayerMask HitLayer;

		[BoxGroup( "Damage (right-click to change type)" )]
		[HideReferenceObjectPicker, LabelText( "@GetDamageTypeName()" )]
		[SerializeReference] public IDamageData Damage;

		private string GetDamageTypeName()
		{
			return Damage == null
				? "Invalid"
				: Damage.HandlerType.GetNiceName().SplitPascalCase();
		}
	}
}
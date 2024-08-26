#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC {

public partial class SoulMovementHandler  : MonoBehaviour
{

	[DrawGizmo(GizmoType.Selected)]
	static void DrawOptimalEgg(SoulMovementHandler target, GizmoType gizmoType)
	{
		if (!target._isInSoulState)
		{
			return;
		}

		if (target._optimalEgg == null)
		{
			return;
		}

		var eggPos = target._optimalEgg.transform.position;
		var pivot = target.transform.position;

		Handles.color = Color.cyan;
		Handles.DrawDottedLine(eggPos, pivot, 2f);
	}
}

}

#endif
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC
{

public partial class BrokenEggLifecycleHandler : MonoBehaviour
{
	[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
	static void DrawBrokenEggGizmos(BrokenEggLifecycleHandler target, GizmoType gizmoType)
	{
		if (target._childTransforms == null)
		{
			return;
		}

		var pivotPos = target.transform.position;
		var style = new GUIStyle();
		var gizmoColor = Color.gray;
		Gizmos.color = gizmoColor;

		style.alignment = TextAnchor.MiddleCenter;
		style.normal.textColor = gizmoColor;

		Handles.Label(pivotPos, $"{target.gameObject.name}", style);

		foreach(var childTransform in target._childTransforms)
		{
			var targetPos = childTransform.position;
			Gizmos.DrawLine(pivotPos, targetPos);
		}
	}

}

}

#endif
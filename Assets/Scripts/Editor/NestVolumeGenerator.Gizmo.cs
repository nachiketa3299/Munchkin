#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC
{

public partial class NestVolumeGenerator : MonoBehaviour
{
	[DrawGizmo(GizmoType.Selected | GizmoType.InSelectionHierarchy)]
	static void DrawGeneratedNestVolumeGizmos(NestVolumeGenerator target, GizmoType gizmoType)
	{
		var gizmoColor = Color.yellow;

		Gizmos.DrawWireCube(target.transform.position + target._seedColliderCenter, target._seedColliderSize);

		var bounds = new Bounds[] {target.LeftBounds(), target.UpBounds(), target.RightBounds(), target.DownBounds() };

		gizmoColor = Color.green;
		gizmoColor.a = 0.2f;
		Gizmos.color = gizmoColor;

		foreach(var bound in bounds)
		{
			Gizmos.DrawCube(target.transform.position + bound.center, bound.size);
		}
	}
}

}

#endif
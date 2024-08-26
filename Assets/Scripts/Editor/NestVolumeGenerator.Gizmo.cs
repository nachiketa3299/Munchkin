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

		var pivot = target.transform.position;

		if (target._up)
		{
			var up = target.UpBounds();
			Gizmos.DrawCube(pivot+ up.center, up.size);
		}

		if (target._left)
		{
			var left = target.LeftBounds();
			Gizmos.DrawCube(pivot+ left.center, left.size);
		}

		if (target._right)
		{
			var right = target.RightBounds();
			Gizmos.DrawCube(pivot+ right.center, right.size);
		}

		if (target._down)
		{
			var down = target.DownBounds();
			Gizmos.DrawCube(pivot+ down.center, down.size);
		}
	}
}

}

#endif
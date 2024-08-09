#if false
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(EggBounds))]
internal sealed class EggBoundsEditor : Editor
{
	[DrawGizmo(GizmoType.Selected | GizmoType.Active)]
	static void DrawEggAllBoundsGizmo(EggBounds target, GizmoType gizmoType)
	{
		Gizmos.color = Color.green;

		foreach (var bound in target.GetAllBounds())
		{
			// Draw each bound
			Gizmos.DrawWireCube(bound.center, bound.size);
		}
	}

	[DrawGizmo(GizmoType.Selected | GizmoType.Active)]
	static void DrawEggCombinedBoundGizmo(EggBounds target, GizmoType gizmoType)
	{
		var combinedBound = target.CalculateCombinedBounds();

		// Draw combined bound

		Gizmos.color = Color.green - new Color(0.0f, 0.0f, 0.0f, 0.3f);
		Gizmos.DrawCube(combinedBound.center, combinedBound.size);

		// Draw combined bound's min & max point

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(combinedBound.max, 0.05f);
		Gizmos.DrawWireSphere(combinedBound.min, 0.05f);
	}
}

}

#endif
#endif
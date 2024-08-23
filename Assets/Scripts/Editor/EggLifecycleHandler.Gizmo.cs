#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC
{
	public partial class EggLifecycleHandler : MonoBehaviour
	{

		[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
		static void DrawUniqueNameOfEgg(EggLifecycleHandler target, GizmoType gizmoType)
		{
			var style = new GUIStyle();
			{
				style.normal.textColor = Color.yellow;
				style.alignment = TextAnchor.MiddleCenter;
			}

			Handles.Label
			(
				position: target.transform.position,
				text: target.gameObject.name,
				style: style
			);
		}
	}
}

#endif
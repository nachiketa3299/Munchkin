#if UNITY_EDITOR

using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace MC {

public partial class SceneLoadTrigger : MonoBehaviour
{
	[DrawGizmo(GizmoType.Selected)]
	static void DrawSceneDependencyByObject(SceneLoadTrigger target, GizmoType gizmoType)
	{
		var data = target._sceneNamesToMaintain;

		if (target._inSceneName == string.Empty)
		{
			return;
		}

		var from = target.transform.position;

		foreach (var sceneName in data)
		{
			var scene = SceneManager.GetSceneByName(sceneName);
			if (!scene.isLoaded)
			{
				continue;
			}

			var to = scene.GetRootGameObjects().First().transform.position;

			Handles.color = Color.gray;
			Handles.DrawDottedLine(from, to, 1f);

			var dir = (to - from).normalized;
			var labelStart = dir * 4.0f;

			var style = new GUIStyle() { alignment = TextAnchor.MiddleCenter };
			style.normal.textColor = Handles.color;

			Handles.Label(from + labelStart, sceneName, style);
		}
	}
}

}

#endif
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC.Editors
{

[CustomEditor(typeof(GrabThrowAction))]
internal sealed class GrabThrowActionEditor : Editor
{

#region UnityCallbacks

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

		var grabThrowAction = target as GrabThrowAction;

		var grabThrowTarget = grabThrowAction.GrabThrowTarget;

		EditorGUI.BeginDisabledGroup(true);

			EditorGUILayout.ObjectField
			(
				label: "Grab Throw Target Object",
				obj: grabThrowTarget,
				objType: typeof(UnityEngine.Object),
				allowSceneObjects: true
			);

		EditorGUI.EndDisabledGroup();

		if (EditorApplication.isPlaying)
		{
			if (GUILayout.Button("Call EndAction"))
			{
				grabThrowAction.EndAction();
			}
		}

		serializedObject.ApplyModifiedProperties();
	}

#endregion // UnityCallbacks

}

}

#endif
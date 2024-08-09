#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(RuntimeLoadedSceneData))]
internal sealed class RuntimeLoadedSceneDataEditor : Editor
{
	#region UnityCallbacks

	public override bool RequiresConstantRepaint() => true;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

		var runtimeLoadedSceneData = target as RuntimeLoadedSceneData;

		var dict = runtimeLoadedSceneData.LoadedScenesByGameObject;

		if (dict.Count == 0)
		{
			return;
		}

		EditorGUILayout.LabelField("Loaded Scenes by GameObject");
		var ret = string.Empty;

		foreach(var (objName, sceneNameSets) in dict)
		{
			ret += $"<b>{objName}</b>:\n\t{string.Join(',', sceneNameSets)}\n";
		}

		var style = new GUIStyle(EditorStyles.textArea)
		{
			richText = true
		};

		EditorGUILayout.BeginScrollView(new(), GUILayout.Height(300.0f));
		EditorGUILayout.SelectableLabel(ret, style, GUILayout.ExpandHeight(true));
		EditorGUILayout.EndScrollView();

		serializedObject.ApplyModifiedProperties();
	}

	#endregion // UnityCallbacks
}

}

#endif
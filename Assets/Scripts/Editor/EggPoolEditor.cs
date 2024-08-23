#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(EggPool))]
internal sealed class EggPoolEditor : PoolBaseEditor
{

#region UnityCallbacks

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

		// Draw active nest egg info box

		EditorGUILayout.LabelField("Active Nest Eggs", EditorStyles.boldLabel);

		var nestEggs = serializedObject.FindProperty("_nestEggs");

		EditorGUI.indentLevel++;

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUI.BeginDisabledGroup(disabled: true);

		if (nestEggs.arraySize == 0)
		{
			EditorGUILayout.LabelField("No active nest eggs");
		}
		else
		{
			for (var i = 0; i < nestEggs.arraySize; ++i)
			{
				EditorGUILayout.ObjectField(nestEggs.GetArrayElementAtIndex(i));
			}
		}

		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();
		EditorGUI.indentLevel--;

		// Draw active character egg  info box

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Active Character Eggs", EditorStyles.boldLabel);

		var characterEggs = serializedObject.FindProperty("_characterEggs");

		EditorGUI.indentLevel++;

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUI.BeginDisabledGroup(disabled: true);

		if (characterEggs.arraySize == 0)
		{
			EditorGUILayout.LabelField("No active character eggs");
		}
		else
		{
			for (var i = 0; i < characterEggs.arraySize; ++i)
			{
				EditorGUILayout.ObjectField(characterEggs.GetArrayElementAtIndex(i));
			}
		}

		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();
		EditorGUI.indentLevel--;

		serializedObject.ApplyModifiedProperties();

	}

#endregion

}

}

#endif
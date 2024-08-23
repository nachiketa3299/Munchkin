#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(BrokenEggPool))]
internal sealed class BrokenEggPoolEditor : PoolBaseEditor
{

#region UnityCallbacks

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		serializedObject.Update();

		EditorGUILayout.LabelField("Active Broken Eggs", EditorStyles.boldLabel);
		var brokenEggs = serializedObject.FindProperty("_brokenEggs");

		EditorGUI.indentLevel++;
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUI.BeginDisabledGroup(disabled: true);

		if (brokenEggs.arraySize == 0)
		{
			EditorGUILayout.LabelField("No active broken eggs");
		}
		else
		{
			for (var i = 0; i < brokenEggs.arraySize; ++i)
			{
				EditorGUILayout.BeginVertical();

				var element = brokenEggs.GetArrayElementAtIndex(i);

				EditorGUILayout.ObjectField(element);

				// I don't know what I am doing
				var obj = new SerializedObject(element.objectReferenceValue);
				var current = obj.FindProperty("_currentLifespan").floatValue;
				var max = obj.FindProperty("_maxLifespan").floatValue;

				EditorGUILayout.FloatField("Lifespan", Mathf.Clamp01(current / max));

				EditorGUILayout.EndVertical();
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
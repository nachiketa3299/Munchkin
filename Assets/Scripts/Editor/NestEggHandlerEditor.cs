#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(NestEggHandler))]
internal sealed class NestEggHandlerEditor : Editor
{

#region UnityCallbacks

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

// See `_nestEggs` elements

		EditorGUILayout.LabelField("Nest eggs:");
		var nestEggBounds = serializedObject.FindProperty("_nestEggs");

		EditorGUI.indentLevel++;

		if (nestEggBounds == null)
		{
			EditorGUILayout.LabelField("nestEggBounds is null");
		}
		else if (nestEggBounds.arraySize == 0)
		{
			EditorGUILayout.LabelField("There is no nest eggs");
		}
		else
		{
			for (var i = 0; i < nestEggBounds.arraySize; ++i)
			{
				var element = nestEggBounds
					.GetArrayElementAtIndex(i)
					.objectReferenceValue
					as EggLifecycleHandler;

				if (element)
				{
					EditorGUILayout.LabelField($"{i:D4}: {element.gameObject.name}");
				}
				else
				{
					EditorGUILayout.LabelField($"{i:D4}: Error");
				}
			}
		}

// Spawn nest egg button

		EditorGUI.indentLevel--;

		GUI.enabled = EditorApplication.isPlaying;

		EditorGUILayout.Space();

		if (GUILayout.Button("Spawn egg in nest"))
		{
			var nest = target as NestEggHandler;
			// nest.SpawnNestEggs(1);
			nest.SendMessage("SpawnNestEggs", 1);
		}

		GUI.enabled = true;

		serializedObject.ApplyModifiedProperties();
	}

#endregion // UnityCallbacks

}

}

#endif
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(NestEggHandler))]
internal sealed class NestEggHandlerEditor : Editor
{
	[DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
	static void DrawGizmoForNestEggHandler(NestEggHandler nestEggHandler, GizmoType gizmoType)
	{
		var spawnPositionObject = (new SerializedObject(nestEggHandler)).FindProperty("_spawnPositionObject").objectReferenceValue as GameObject;
		Gizmos.DrawWireCube(spawnPositionObject.transform.position, 1.0f * Vector3.one);
	}

	#region UnityCallbacks

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();


		EditorGUILayout.LabelField("Nest eggs:");
		var nestEggs = serializedObject.FindProperty("_nestEggs");

		EditorGUI.indentLevel++;

		if (nestEggs.arraySize == 0)
		{
			EditorGUILayout.LabelField("List is empty");
		}
		else
		{
			for (var i = 0; i < nestEggs.arraySize; ++i)
			{
				var element = nestEggs.GetArrayElementAtIndex(i);
				EditorGUILayout.LabelField($"{i}: {element}");
			}
		}

		EditorGUI.indentLevel--;


		if (EditorApplication.isPlaying)
		{
			EditorGUILayout.Space();

			if (GUILayout.Button("Spawn egg in nest"))
			{
				var nest = target as NestEggHandler;
				nest.SpawnEggInNest();
			}
		}

		serializedObject.ApplyModifiedProperties();
	}

	#endregion // UnityCallbacks

}

}

#endif
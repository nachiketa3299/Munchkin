#if UNITY_EDITOR

using UnityEditor;
using UnityEngine.Pool;

namespace MC.Editors
{

[CustomEditor(typeof(RuntimePooledEggData))]
internal sealed class RuntimePooledEggDataEditor : Editor
{

#region UnityCallbacks

	public override bool RequiresConstantRepaint() => true;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

		var data = target as RuntimePooledEggData;

		// Character egg pool state

		if (data.Pool == null)
		{
			EditorGUILayout.LabelField("EggPool is not initialized");
		}
		else
		{
			EditorGUILayout.LabelField("EggPool data:");
			DrawDisabledGroup(data.Pool);
		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("NestEggs:");
		var nestEggs = serializedObject.FindProperty("_nestEggs");

		EditorGUI.indentLevel++;
		if (nestEggs == null)
		{
			EditorGUILayout.LabelField("null");
		}
		else if (nestEggs.arraySize == 0)
		{
			EditorGUILayout.LabelField("There is no active nest eggs");
		}
		else
		{
			for (var i = 0; i < nestEggs.arraySize; ++i)
			{
				var element = nestEggs
					.GetArrayElementAtIndex(i)
					.objectReferenceValue
					as EggLifecycleHandler;
				if (element)
				{
					EditorGUILayout.LabelField($"{i:D4}: {element.gameObject.name}");
				}
				else
				{
					EditorGUILayout.LabelField($"{i:D4} Error");
				}
			}
		}
		EditorGUI.indentLevel--;

		EditorGUILayout.LabelField("CharacterEggs:");
		var characterEggs = serializedObject.FindProperty("_characterEggs");

		EditorGUI.indentLevel++;
		if (characterEggs == null)
		{
			EditorGUILayout.LabelField("null");
		}
		else if (characterEggs.arraySize == 0)
		{
			EditorGUILayout.LabelField("There is no active character eggs");
		}
		else
		{
			for (var i = 0; i < characterEggs.arraySize; ++i)
			{
				var element = characterEggs
					.GetArrayElementAtIndex(i)
					.objectReferenceValue
					as EggLifecycleHandler;
				if (element)
				{
					EditorGUILayout.LabelField($"{i:D4}: {element.gameObject.name}");
				}
				else
				{
					EditorGUILayout.LabelField($"{i:D4} Error");
				}
			}
		}
		EditorGUI.indentLevel--;

		serializedObject.ApplyModifiedProperties();
	}

#endregion // UnityCallbacks

	void DrawDisabledGroup(ObjectPool<EggLifecycleHandler> pool)
	{
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUI.BeginDisabledGroup(true);

		EditorGUILayout.IntField("CountAll", pool.CountAll);
		EditorGUILayout.IntField("CountActive", pool.CountActive);
		EditorGUILayout.IntField("CountInActive", pool.CountInactive);

		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();
	}
}

}


#endif
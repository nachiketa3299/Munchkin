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
			EditorGUILayout.LabelField("CharacterEggPool is not initialized");
		}
		else
		{
			EditorGUILayout.LabelField("CharacterEggPool data:");
			DrawDisabledGroup(data.Pool);
		}

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
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace MC.Editors
{


[CustomEditor(typeof(PoolBase<>), true)]
internal class PoolBaseEditor : Editor
{

#region UnityCallbacks
	public override bool RequiresConstantRepaint() => true;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

		if (target is PoolBase<EggLifecycleHandler> eggPool)
		{
			if (eggPool.Pool == null)
			{
				return;
			}

			_countAll = eggPool.Pool.CountAll;
			_countActive = eggPool.Pool.CountActive;
			_countInActive = eggPool.Pool.CountInactive;
		}

		else if (target is PoolBase<BrokenEggLifecycleHandler> brokenEggPool)
		{
			if (brokenEggPool.Pool == null)
			{
				return;
			}

			_countAll = brokenEggPool.Pool.CountAll;
			_countActive = brokenEggPool.Pool.CountActive;
			_countInActive = brokenEggPool.Pool.CountInactive;
		}

		EditorGUILayout.LabelField("Pool Data", EditorStyles.boldLabel);

		EditorGUI.indentLevel++;
		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUI.BeginDisabledGroup(disabled: true);

		EditorGUILayout.IntField("All", _countAll);
		EditorGUILayout.IntField("Active", _countActive);
		EditorGUILayout.IntField("Inactive", _countInActive);

		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();
		EditorGUI.indentLevel--;

		serializedObject.ApplyModifiedProperties();
	}

#endregion // UnityCallbacks

		int _countAll;
		int _countActive;
		int _countInActive;

	}


}

#endif
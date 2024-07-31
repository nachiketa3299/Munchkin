#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEngine.Pool;


namespace MC
{
	[CustomEditor(typeof(RuntimePooledEggData))]
	public class RuntimePooledEggDataEditor : Editor
	{
		#region UnityCallbacks

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			base.OnInspectorGUI();

			var data = target as RuntimePooledEggData;

			if (data.Pool == null)
			{
				EditorGUILayout.LabelField("Pool is not initialized");
			}

			else
			{
				EditorGUILayout.LabelField("Pool data:");
				var all = data.Pool.CountAll;
				var active = data.Pool.CountActive;
				var inactive = data.Pool.CountInactive;

				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUI.BeginDisabledGroup(true);

				EditorGUILayout.IntField("CountAll", all);
				EditorGUILayout.IntField("CountActive", active);
				EditorGUILayout.IntField("CountInActive", inactive);

				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();
			}
		}
		#endregion // UnityCallbacks
	}
}


#endif
#if UNITY_EDITOR

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

			// Character egg pool state

			if (data.CharacterEggPool == null)
			{
				EditorGUILayout.LabelField("CharacterEggPool is not initialized");
			}
			else
			{
				EditorGUILayout.LabelField("CharacterEggPool data:");
				DrawDisabledGroup(data.CharacterEggPool);
			}

			// Nest egg pool state

			if (data.NestEggPool == null)
			{
				EditorGUILayout.LabelField("NestEggPool is not initialized");
			}
			else
			{
				EditorGUILayout.LabelField("NestEggPool data:");
				DrawDisabledGroup(data.NestEggPool);
			}
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
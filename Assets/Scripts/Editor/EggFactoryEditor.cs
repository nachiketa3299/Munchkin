#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC
{

	public partial class EggFactory : MonoBehaviour
	{
		[CustomEditor(typeof(EggFactory))]
		private class EggFactoryEditor : Editor
		{
			// public override bool RequiresConstantRepaint() => true;
			void OnEnable()
			{
				_eggFactory = target as EggFactory;

				EditorApplication.update += Repaint;
			}

			void OnDisable()
			{
				EditorApplication.update -= Repaint;
			}

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				if (!_eggFactory)
				{
					return;
				}

				if (!Application.isPlaying)
				{
					return;
				}

				EditorGUILayout.Space();

				if (!_eggFactory._eggPool.IsPoolInitialized)
				{
					EditorGUILayout.LabelField("Egg pool is not initialized");
					return;
				}

				var countAll = _eggFactory._eggPool.CountAll;
				var countActive = _eggFactory._eggPool.CountActive;
				var countInactive = _eggFactory._eggPool.CountInactive;

				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUI.BeginDisabledGroup(true);

				EditorGUILayout.IntField("CountAll", countAll);
				EditorGUILayout.IntField("CountActive", countActive);
				EditorGUILayout.IntField("CountInActive", countInactive);

				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();

				Repaint();
			}

			EggFactory _eggFactory;
		}
	}

}

#endif
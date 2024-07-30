#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;

namespace MC
{

	public partial class RuntimePooledEggData : ScriptableObject
	{
		public bool IsPoolInitialized => _pool != null;
		public int CountInactive => _pool.CountInactive;
		public int CountActive => _pool.CountActive;
		public int CountAll => _pool.CountAll;


		[CustomEditor(typeof(RuntimePooledEggData))]
		private class RuntimePooledEggDataEditor : Editor
		{
			#region UnityCallbacks

			void OnEnable()
			{
				_runtimePooledEggData = target as RuntimePooledEggData;
			}

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				if (!_runtimePooledEggData)
				{
					return;
				}

				if (GUILayout.Button("Calculate Egg Prefab Bound"))
				{
					if (!_runtimePooledEggData._eggPrefab)
					{
						Debug.LogError("Egg Prefab이 할당되지 않았습니다.");
						return;
					}

					// var tempObject = PrefabUtility.InstantiatePrefab(_runtimePooledEggData._eggPrefab) as GameObject;
					// if (tempObject == null)
					// {
					// 	Debug.LogError("Egg Prefab 인스턴싱을 실패하였습니다.");
					// 	DestroyImmediate(tempObject);
					// 	return;
					// }

					var prefabStage = PrefabStageUtility.OpenPrefab
					(
						AssetDatabase.GetAssetPath(_runtimePooledEggData._eggPrefab)
					);

					if (prefabStage == null)
					{
						Debug.LogError("Egg Prefab 스테이지를 열 수 없습니다.");
						return;
					}

					var tempObject = prefabStage.prefabContentsRoot;

					var physicalColliders = tempObject.transform.root.gameObject.GetComponentsInChildren<Collider>()
						.Where(collider => !collider.isTrigger)
						.ToArray();

					if (physicalColliders.Length == 0)
					{
						Debug.LogWarning("Egg Prefab 에서 어떤 물리 콜라이더도 발견되지 않았습니다.");
						return;
					}

					var combinedBounds = physicalColliders[0].bounds;

					for (var i = 1; i < physicalColliders.Length; ++i)
					{
						combinedBounds.Encapsulate(physicalColliders[i].bounds);
					}

					Debug.Log($"Egg 의 물리 복합 바운드를 계산하였습니다: ({combinedBounds.extents.x}, {combinedBounds.extents.y}, {combinedBounds.extents.z})");

					// Set RuntimePooledEggData

					_runtimePooledEggData.EggCombinedPhysicalBounds = combinedBounds;

					EditorUtility.SetDirty(_runtimePooledEggData);
					AssetDatabase.SaveAssets();

					Debug.Log($"Egg 의 물리 복합 바운드 정보가 성공적으로 {_runtimePooledEggData}에 저장되었습니다.");

					StageUtility.GoBackToPreviousStage();
				}

				EditorGUILayout.Space();

				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Vector3Field
				(
					label: "Physical Bounds Extents",
					value: _runtimePooledEggData.EggCombinedPhysicalBounds.extents
				);
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();
			}

			#endregion

			RuntimePooledEggData _runtimePooledEggData;
		}
	}
}

#endif
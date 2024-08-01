using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
	/// <summary>
	/// 여러 오브젝트들에 의해 로드/언로드되어야 하는 씬들을 관리
	/// </summary>
	[DisallowMultipleComponent]
	public class SceneLoadManager : MonoBehaviour
	{

		#region Unity Callbacks

		void OnEnable()
		{
			_runtimeLoadedSceneData.SceneOperationNeeded += OnSceneOperationNeeded;
		}

		void OnDisable()
		{
			_runtimeLoadedSceneData.SceneOperationNeeded -= OnSceneOperationNeeded;
		}

		void FixedUpdate()
		{
			_runtimeLoadedSceneData.TryProcessChanges();
		}

		#endregion // Unity Callbacks

		/// <summary>
		/// 어떤 씬들의 연산이 필요성이 수신되었을때 실행되는 로직
		/// </summary>
		void OnSceneOperationNeeded(HashSet<string> uniqueSceneNamesToLoad, HashSet<string> uniqueSceneNamesToUnload)
		{

#if UNITY_EDITOR
			if (_logOnSceneOperation)
			{
				var toLoadSceneNames = string.Join(", ", uniqueSceneNamesToLoad);
				var toUnloadSceneNames = string.Join(", ", uniqueSceneNamesToUnload);

				Debug.Log
				(
$@"Scene operations needed:
	To load: <color=green>{toLoadSceneNames}</color>
	To unload: <color=red>{toUnloadSceneNames}</color>"
				);
			}
#endif

			StartCoroutine(ProcessSceneOperationsRoutine(uniqueSceneNamesToLoad, uniqueSceneNamesToUnload));
		}

		/// <remarks>
		/// 어떤 씬들의 연산이 필요하다고 바로 그 연산이 수행되는 것이 아님. 해당 씬에 대한 연산이 이미 진행 중이라면, 무시한다.
		/// </remarks>
		IEnumerator ProcessSceneOperationsRoutine(HashSet<string> uniqueSceneNamesToLoad, HashSet<string> uniqueSceneNamesToUnload)
		{
			// 언로딩 연산에 대한 처리

			foreach (var sceneName in uniqueSceneNamesToUnload)
			{
				if (AlreadyUnloading(sceneName))
				{

#if UNITY_EDITOR
					if (_logOnSceneOperation)
					{
						Debug.Log($"<color=yellow>Scene {sceneName} is already in loading process, so ignored load request.</color>");
					}
#endif

					continue;
				}

				_unloadingSceneNames.Add(sceneName);

				yield return StartCoroutine(UnloadSceneRoutine(sceneName));
			}

			// 로딩 연산에 대한 처리

			foreach (var sceneName in uniqueSceneNamesToLoad)
			{
				if (AlreadyLoading(sceneName))
				{

#if UNITY_EDITOR
					if (_logOnSceneOperation)
					{
						Debug.Log($"<color=yellow>Scene {sceneName} is already in unloading process, so ignored load request.</color>");
					}
#endif
					continue;
				}

				_loadingSceneNames.Add(sceneName);

				yield return StartCoroutine(LoadSceneRoutine(sceneName));
			}
		}

		IEnumerator UnloadSceneRoutine(string sceneName)
		{

#if UNITY_EDITOR
			if (_logOnSceneOperation)
			{
				Debug.Log($"Start loading scene {sceneName}");
			}
#endif

			var operation = SceneManager.UnloadSceneAsync(sceneName);

			while (!operation.isDone)
			{
				yield return operation;
			}

			_unloadingSceneNames.Remove(sceneName);

#if UNITY_EDITOR
			if (_logOnSceneOperation)
			{
				Debug.Log($"End Unloading {sceneName}");
			}
#endif

		}

		IEnumerator LoadSceneRoutine(string sceneName)
		{

#if UNITY_EDITOR
			if (_logOnSceneOperation)
			{
				Debug.Log($"Start unloading scene {sceneName}");
			}
#endif
			var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			while (!operation.isDone)
			{
				yield return operation;
			}

			_loadingSceneNames.Remove(sceneName);

#if UNITY_EDITOR
			if (_logOnSceneOperation)
			{
				Debug.Log($"End unloading scene {sceneName}");
			}
#endif
		}

		bool AlreadyUnloading(string sceneName) => _unloadingSceneNames.Contains(sceneName);

		bool AlreadyLoading(string sceneName) => _loadingSceneNames.Contains(sceneName);

		/// <summary>
		/// 현재 로드되는 중인 씬들의 이름
		/// </summary>
		HashSet<string> _loadingSceneNames = new();

		/// <summary>
		/// 현재 언로드되는 중인 씬들의 이름
		/// </summary>
		HashSet<string> _unloadingSceneNames = new();

		[SerializeField] RuntimeLoadedSceneData _runtimeLoadedSceneData;

#if UNITY_EDITOR
		[SerializeField] bool _logOnSceneOperation = false;
#endif
	}
}
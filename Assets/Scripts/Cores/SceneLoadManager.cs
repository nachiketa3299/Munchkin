using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
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
			_runtimeLoadedSceneData.ProcessChanges();
		}

		#endregion // Unity Callbacks

		void OnSceneOperationNeeded(HashSet<string> uniqueSceneNamesToLoad, HashSet<string> uniqueSceneNamesToUnload)
		{
			StartCoroutine(ProcessSceneOperationsRoutine(uniqueSceneNamesToLoad, uniqueSceneNamesToUnload));
		}

		IEnumerator ProcessSceneOperationsRoutine(HashSet<string> uniqueSceneNamesToLoad, HashSet<string> uniqueSceneNamesToUnload)
		{
			// Unloading Scenes
			foreach (var sceneName in uniqueSceneNamesToUnload)
			{
				if (_unloadingSceneNames.Contains(sceneName))
				{
					continue;
				}

				_unloadingSceneNames.Add(sceneName);

				yield return StartCoroutine(UnloadSceneRoutine(sceneName));
			}

			// Load Scenes
			foreach (var sceneName in uniqueSceneNamesToLoad)
			{
				if (_loadingSceneNames.Contains(sceneName))
				{
					continue;
				}

				_loadingSceneNames.Add(sceneName);

				yield return StartCoroutine(LoadSceneRoutine(sceneName));
			}
		}

		IEnumerator UnloadSceneRoutine(string sceneName)
		{
			Debug.Log($"Start Unloading {sceneName}");

			var operation = SceneManager.UnloadSceneAsync(sceneName);

			while (!operation.isDone)
			{
				yield return operation;
			}

			Debug.Log($"End Unloading {sceneName}");

			_unloadingSceneNames.Remove(sceneName);
		}

		IEnumerator LoadSceneRoutine(string sceneName)
		{
			Debug.Log($"Start Loading {sceneName}");

			var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			while (!operation.isDone)
			{
				yield return operation;
			}

			Debug.Log($"End Loading {sceneName}");

			_loadingSceneNames.Remove(sceneName);
		}

		[SerializeField] RuntimeLoadedSceneData _runtimeLoadedSceneData;

		HashSet<string> _loadingSceneNames = new();
		HashSet<string> _unloadingSceneNames = new();
	}
}
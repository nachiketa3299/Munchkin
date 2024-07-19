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
			foreach (var sceneName in uniqueSceneNamesToUnload.ToList())
			{
				if (SceneManager.GetSceneByName(sceneName).isLoaded && !_unloadingSceneNames.Contains(sceneName))
				{
					_unloadingSceneNames.Add(sceneName);
					yield return StartCoroutine(UnloadSceneRoutine(sceneName));
				}
			}

			// Load Scenes
			foreach (var sceneName in uniqueSceneNamesToLoad.ToList())
			{
				if (!SceneManager.GetSceneByName(sceneName).isLoaded && !_loadingSceneNames.Contains(sceneName))
				{
					_loadingSceneNames.Add(sceneName);
					yield return StartCoroutine(LoadSceneRoutine(sceneName));
				}
			}
		}

		IEnumerator UnloadSceneRoutine(string sceneName)
		{
			var operation = SceneManager.UnloadSceneAsync(sceneName);
			yield return operation;
			_unloadingSceneNames.Remove(sceneName);
		}

		IEnumerator LoadSceneRoutine(string sceneName)
		{
			var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			yield return operation;
			_loadingSceneNames.Remove(sceneName);
		}

		[SerializeField] RuntimeLoadedSceneData _runtimeLoadedSceneData;

		HashSet<string> _loadingSceneNames = new();
		HashSet<string> _unloadingSceneNames = new();
	}
}
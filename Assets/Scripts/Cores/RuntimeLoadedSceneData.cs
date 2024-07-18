using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
	[CreateAssetMenu(fileName = "RuntimeLoadedSceneData", menuName = "MC/Scriptable Objects/Runtime Loaded Scene Data")]
	public class RuntimeLoadedSceneData : ScriptableObject
	{
		public event Action<IEnumerable<AsyncOperation>> SceneOperationNeeded;

		public void EnteredNewScene(GameObject triggeredObject, string enteredSceneName, int depthToLoad)
		{
			var nearSceneUniqueNames = _sceneDependencyData.RetrieveNearSceneUniqueNames(enteredSceneName, depthToLoad);
			var prevLoadedSceneNames = new HashSet<string>();

			if (!_loadedScenesByGameObject.ContainsKey(triggeredObject))
			{
				_loadedScenesByGameObject.Add(triggeredObject, nearSceneUniqueNames);
			}
			else
			{
				prevLoadedSceneNames = _loadedScenesByGameObject[triggeredObject];
				_loadedScenesByGameObject[triggeredObject] = nearSceneUniqueNames;
			}

			var allSceneNamesShouldBeLoaded = _loadedScenesByGameObject.Values.SelectMany((_) => (_)).ToHashSet();
			var toUnloadByTriggeredObject = prevLoadedSceneNames.Except(allSceneNamesShouldBeLoaded);

			Debug.Log($"ToLoad by {triggeredObject}");
			foreach (var name in allSceneNamesShouldBeLoaded)
			{
				Debug.Log(name);
			}

			Debug.Log($"ToUnload by {triggeredObject}");
			foreach (var name in toUnloadByTriggeredObject)
			{
				Debug.Log(name);
			}

			SceneOperationNeeded?.Invoke(LoadScenesAsync(allSceneNamesShouldBeLoaded).Concat(UnloadScenesAsync(toUnloadByTriggeredObject)));
		}

		IEnumerable<AsyncOperation> LoadScenesAsync(IEnumerable<string> sceneNames)
		{
			var operations = new List<AsyncOperation>();

			foreach (var sceneName in sceneNames)
			{
				if (SceneManager.GetSceneByName(sceneName).isLoaded)
				{
					continue;
				}

				operations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
			}

			return operations;
		}

		IEnumerable<AsyncOperation> UnloadScenesAsync(IEnumerable<string> sceneNames)
		{
			var operations = new List<AsyncOperation>();

			foreach (var sceneName in sceneNames)
			{
				if (!SceneManager.GetSceneByName(sceneName).isLoaded)
				{
					continue;
				}

				operations.Add(SceneManager.UnloadSceneAsync(sceneName));
			}

			return operations;
		}

		Dictionary<GameObject, HashSet<string>> _loadedScenesByGameObject = new();

		[SerializeField] SceneDependencyData _sceneDependencyData;
	}
}
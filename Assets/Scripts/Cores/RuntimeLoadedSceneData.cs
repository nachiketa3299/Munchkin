using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace MC
{
	[CreateAssetMenu(fileName = "RuntimeLoadedSceneData", menuName = "MC/Scriptable Objects/Runtime Loaded Scene Data")]
	public class RuntimeLoadedSceneData : ScriptableObject
	{
		public event Action<IEnumerable<string>> AsyncLoadSceneOperationNeeded;
		public event Action<IEnumerable<string>> AsyncUnloadSceneOperationNeeded;

		public void OnEnteredNewScene(GameObject objectEntering, string enteredSceneName, int depthToLoad)
		{
			var nearSceneUniqueNames = _sceneDependencyData.RetrieveNearSceneUniqueNames(enteredSceneName, depthToLoad);
			var prevLoadedSceneNames = new HashSet<string>();

			if (!_loadedScenesByGameObject.ContainsKey(objectEntering))
			{
				_loadedScenesByGameObject.Add(objectEntering, nearSceneUniqueNames);
			}
			else
			{
				prevLoadedSceneNames = _loadedScenesByGameObject[objectEntering];
				_loadedScenesByGameObject[objectEntering] = nearSceneUniqueNames;
			}

			var allUniqueSceneNamesShouldBeLoaded = _loadedScenesByGameObject.Values.SelectMany((_) => (_)).ToHashSet();
			var allUniqueSceneNamesShouldBeUnloaded = prevLoadedSceneNames.Except(allUniqueSceneNamesShouldBeLoaded);

			AsyncLoadSceneOperationNeeded?.Invoke(allUniqueSceneNamesShouldBeLoaded);
			AsyncUnloadSceneOperationNeeded?.Invoke(allUniqueSceneNamesShouldBeUnloaded);
		}

		Dictionary<GameObject, HashSet<string>> _loadedScenesByGameObject = new();
		[SerializeField] SceneDependencyData _sceneDependencyData;
	}
}
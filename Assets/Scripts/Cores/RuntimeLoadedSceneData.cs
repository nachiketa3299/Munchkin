using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace MC
{
	/// <summary>
	/// <see cref="SceneLoadManager"/> 가 부착된 오브젝트가 현재 어떤 씬을 로드하고 있는지 관리하는 싱글턴 데이터
	/// </summary>
	[CreateAssetMenu(fileName = "RuntimeLoadedSceneData", menuName = "MC/Scriptable Objects/Runtime Loaded Scene Data")]
	public class RuntimeLoadedSceneData : ScriptableObject
	{
		public event Action<HashSet<string>, HashSet<string>> SceneOperationNeeded;

		public void OnEnteredNewScene(GameObject objectEntering, string enteredSceneName, int depthToLoad)
		{
			var nearSceneUniqueNames = _sceneDependencyData.RetrieveNearSceneUniqueNames(enteredSceneName, depthToLoad);

			if (!_loadedScenesByGameObject.ContainsKey(objectEntering)) // 이 오브젝트에 대해서는 처음
			{
				_loadedScenesByGameObject.Add(objectEntering, new(nearSceneUniqueNames));
			}
			else // 이미 로드를 했던 적이 있다
			{
				var prevLoadedSceneNames = _loadedScenesByGameObject[objectEntering];
				// 이전에 로드한 씬 중에서, 앞으로 로드할 씬을 빼면, 순수하게 이 오브젝트에 대해 언로드할 씬만 남음

				_pendingUnloadSceneNames.UnionWith(prevLoadedSceneNames.Except(nearSceneUniqueNames));
				_loadedScenesByGameObject[objectEntering] = new(nearSceneUniqueNames);
			}

			_pendingLoadSceneNames.UnionWith(nearSceneUniqueNames);
			_hasChanges = true;
		}

		/// <summary>
		/// <see cref="SceneLoadManager"/> 가 매 프레임 이 메서드를 호출하면서
		/// <see cref="_pendingLoadSceneNames"/> 와 <see cref="_pendingUnLoadSceneNames"/> 에 변경 사항이 있으면 새 씬 관련 연산이 필요하다는
		/// 이벤트(<see cref="SceneOperationNeeded"/> )를 발생시킨다.
		/// </summary>
		public void ProcessChanges()
		{
			if (_hasChanges)
			{
				// 각 오브젝트 별로 로드해야 하는 씬들을 모두 합친다
				var allLoadedScenes = _loadedScenesByGameObject.Values.SelectMany(_ => _).ToHashSet();

				// 그리고, 언로드되어야 하는 씬 중에서, 언로드되면 안 되는 씬들을 뺀다.
				// (다른 오브젝트가 존재하고 있기 때문에 언로드되면 안 되는 경우가 있다.)
				_pendingUnloadSceneNames.ExceptWith(allLoadedScenes);

				SceneOperationNeeded?.Invoke(_pendingLoadSceneNames, _pendingUnloadSceneNames);

				_pendingLoadSceneNames.Clear();
				_pendingUnloadSceneNames.Clear();
				_hasChanges = false;
			}
		}

		[SerializeField] SceneDependencyData _sceneDependencyData;
		Dictionary<GameObject, HashSet<string>> _loadedScenesByGameObject = new();

		HashSet<string> _pendingLoadSceneNames = new();
		HashSet<string> _pendingUnloadSceneNames = new();
		bool _hasChanges = false;
	}
}
using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
	/// <summary>
	/// <see cref="SceneLoadManager"/> 가 부착된 오브젝트가 현재 어떤 씬을 로드하고 있는지 관리하는 싱글턴 데이터
	/// </summary>
	[CreateAssetMenu(fileName = "RuntimeLoadedSceneData", menuName = "MC/Scriptable Objects/Runtime Loaded Scene Data")]
	public class RuntimeLoadedSceneData : ScriptableObject
	{
		public event Action<HashSet<string>, HashSet<string>> SceneOperationNeeded;

		public void OnEnteredNewScene(GameObject enteringObject, string enteredSceneName, int depthToLoad)
		{
			// 앞으로 이 오브젝트에 의해 유지해야 할 씬 목록
			var nearSceneUniqueNamesByObject = _sceneDependencyData.RetrieveNearSceneUniqueNames(enteredSceneName, depthToLoad);

			// 이전에 이 오브젝트에 대한 정보가 없는 경우
			if (!_loadedScenesByGameObject.TryGetValue(enteringObject, out var prevLoadedSceneNamesByObject))
			{
				_loadedScenesByGameObject.Add(enteringObject, nearSceneUniqueNamesByObject);
			}
			// 있는 경우 (prevLoadedSceneNamesByObject)
			else
			{
				_loadedScenesByGameObject[enteringObject] = new HashSet<string>(nearSceneUniqueNamesByObject);
			}

			// 무언가 변경되었으며, 처리가 필요함을 Notate 함
			_hasChanges = true;
		}

		/// <summary>
		/// <see cref="SceneLoadManager"/> 가 매 프레임 이 메서드를 호출하면서
		/// <see cref="_pendingLoadSceneNames"/> 와 <see cref="_pendingUnLoadSceneNames"/> 에 변경 사항이 있으면 새 씬 관련 연산이 필요하다는
		/// 이벤트(<see cref="SceneOperationNeeded"/> )를 발생시킨다.
		/// </summary>
		public void ProcessChanges()
		{
			if (!_hasChanges)
			{
				return;
			}

			// 오브젝트들을 위해 필요한 모든 씬의 목록
			var allRequiredSceneNamesByObjects = _loadedScenesByGameObject.Values.SelectMany(_ => _).ToHashSet();

			// 현재 이미 로드되어 있는 씬들의 목록
			var currentlyLoadedSceneNames = RetrieveAllLoadedSceneNames;

			// 현재 로드되어 있는 씬 목록에서 현재 필요한 목록을 빼면, 로드되어 있는데 필요 없는 목록임
			_pendingUnloadSceneNames.UnionWith(currentlyLoadedSceneNames.Except(allRequiredSceneNamesByObjects));

			// 현재 필요한 목록에서 현재 로드되어 있는 씬 목록을 빼면, 로드되어 있지 않은데 필요한 목록임
			_pendingLoadSceneNames.UnionWith(allRequiredSceneNamesByObjects.Except(currentlyLoadedSceneNames));

			// 여기서 반드시 참조가 아닌 복사로 전달해야 함
			SceneOperationNeeded?.Invoke(new(_pendingLoadSceneNames), new(_pendingUnloadSceneNames));

			_pendingLoadSceneNames.Clear();
			_pendingUnloadSceneNames.Clear();

			_hasChanges = false;
		}

		HashSet<string> RetrieveAllLoadedSceneNames
		{
			get
			{
				var ret = new HashSet<string>();
				for (var i = 0; i < SceneManager.sceneCount; ++i)
				{
					var name = SceneManager.GetSceneAt(i).name;

					if (name == _sceneDependencyData.PersistentSceneName)
					{
						continue;
					}

					ret.Add(name);
				}

				return ret;
			}
		}

		[SerializeField] SceneDependencyData _sceneDependencyData;

		public Dictionary<GameObject, HashSet<string>> LoadedScenesByGameObject => _loadedScenesByGameObject;
		Dictionary<GameObject, HashSet<string>> _loadedScenesByGameObject = new();

		/// <summary>
		/// 모든 오브젝트에 대해, 로드(유지)되어야 하는 씬임이 확정된 씬 이름들
		/// </summary>
		HashSet<string> _pendingLoadSceneNames = new();

		/// <summary>
		/// 모든 오브젝트에 대해, 언로드 되어야 하는 씬임이 확정된 씬 이름들
		/// </summary>
		HashSet<string> _pendingUnloadSceneNames = new();

		/// <summary>
		/// <see cref="SceneLoadManager"/>가 매 프레임 이 값을 검사하여 참이면 씬 로딩 상태를 업데이트함
		/// </summary>
		bool _hasChanges = false;
	}
}
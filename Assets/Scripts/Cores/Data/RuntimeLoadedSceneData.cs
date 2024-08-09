using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{

/// <summary>
/// <see cref="SceneLoadTrigger"/> 로부터 씬 로드/언로드 요청을 받아 정리하여, <br/>
/// <see cref="SceneLoadManager"/> 에게 로드/유지/언로드 씬 목록을 보낸다.
/// </summary>
[CreateAssetMenu(fileName = "RuntimeLoadedSceneData", menuName = "MC/Scriptable Objects/Runtime Loaded Scene Data")]
public class RuntimeLoadedSceneData : ScriptableObject
{
	public delegate void SceneOperationNeededHandler(HashSet<string> toLoadSceneNames, HashSet<string> toUnloadSceneNames);
	public event SceneOperationNeededHandler SceneOperationNeeded;

	public void PendingAddSceneData(GameObject enteringGameObject, in string sceneName, in int depthToLoad)
	{
		var sceneNamesToAdd = _sceneDependencyData.RetrieveNearSceneUniqueNames(sceneName, depthToLoad);

		if (!_loadedScenesByGameObject.ContainsKey(enteringGameObject))
		{
			_loadedScenesByGameObject.Add(enteringGameObject, sceneNamesToAdd);
		}
		else
		{
			_loadedScenesByGameObject[enteringGameObject] = sceneNamesToAdd;
		}

		_isDirty = true;
	}

	public void PendingRemoveSceneData(GameObject enteringGameObject)
	{
		_isDirty = _loadedScenesByGameObject.Remove(enteringGameObject);
	}

	/// <summary>
	/// <see cref="SceneLoadManager"/> 가 매 프레임 이 메서드를 호출하면서
	/// <see cref="_pendingLoadSceneNames"/> 와 <see cref="_pendingUnLoadSceneNames"/> 에 변경 사항이 있으면 새 씬 관련 연산이 필요하다는
	/// 이벤트(<see cref="SceneOperationNeeded"/> )를 발생시킨다.
	/// </summary>
	public void TryProcessChanges()
	{
		if (!_isDirty)
		{
			return;
		}

		// 오브젝트들을 위해 필요한 모든 씬의 목록
		var allRequestedSceneNames = _loadedScenesByGameObject.Values.SelectMany(_ => _).ToHashSet();

		// 현재 이미 로드되어 있는 씬들의 목록
		var currentlyLoadedSceneNames = RetrieveAllLoadedSceneNames;

		// 현재 로드되어 있는 씬 목록에서 현재 필요한 목록을 빼면, 로드되어 있는데 필요 없는 목록임
		_pendingUnloadSceneNames.UnionWith(currentlyLoadedSceneNames.Except(allRequestedSceneNames));

		// 현재 필요한 목록에서 현재 로드되어 있는 씬 목록을 빼면, 로드되어 있지 않은데 필요한 목록임
		_pendingLoadSceneNames.UnionWith(allRequestedSceneNames.Except(currentlyLoadedSceneNames));

		// 여기서 반드시 참조가 아닌 복사로 전달해야 함
		SceneOperationNeeded?.Invoke(new(_pendingLoadSceneNames), new(_pendingUnloadSceneNames));

		_pendingLoadSceneNames.Clear();
		_pendingUnloadSceneNames.Clear();

		_isDirty = false;
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

#if UNITY_EDITOR
	public SceneDependencyData SceneDependencyData => _sceneDependencyData;
#endif

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
	bool _isDirty = false;

}

}
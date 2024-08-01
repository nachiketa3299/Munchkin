using UnityEngine;

namespace MC
{
	/// <summary>
	/// 게임 오브젝트가 씬의 로딩 박스에 존재하게 되거나 존재하게 되지 않을 때,<br/>
	/// 주변 씬의 로드/언로드 요청을 보낸다.
	/// </summary>
	[DisallowMultipleComponent]
	public class SceneLoadTrigger : MonoBehaviour
	{
		public delegate void SceneDataRequestAddedHandler(GameObject enteringGameObject, in string sceneName, in int depth);
		public delegate void SceneDataRequestRemovedHandler(GameObject enteringGameObject);

		/// <summary>
		/// 게임 오브젝트가 사라졌으며(파괴, 비활성화), 따라서 이 게임오브젝트가 유지하던 씬 목록이 더이상 필요치 않음을 명시
		/// </summary>
		public event SceneDataRequestAddedHandler SceneDataRequestAdded;

		/// <summary>
		/// 게임 오브젝트가 해당 이름을 가진 씬 로딩 박스에 접촉하였으며, 이 씬으로부터 거리가 일정 이내인 씬들이 로드되어야 함을 명시
		/// </summary>
		public event SceneDataRequestRemovedHandler SceneDataRequestRemoved;

		#region UnityCallbacks

		void Awake()
		{

#if UNITY_EDITOR
			if (!_runtimeLoadedSceneData)
			{
				Debug.LogWarning("RuntimeLoadedSceneData를 찾을 수 없습니다.");
			}
#endif

			// Bind events

			SceneDataRequestAdded += _runtimeLoadedSceneData.PendingAddSceneData;
			SceneDataRequestRemoved = _runtimeLoadedSceneData.PendingRemoveSceneData;
		}

		void OnDisable()
		{
			SceneDataRequestRemoved?.Invoke(gameObject);
		}

		void OnDestroy()
		{
			SceneDataRequestRemoved?.Invoke(gameObject); // 필요한 지는 모르겠으나...

			// Unbind events

			SceneDataRequestAdded -= _runtimeLoadedSceneData.PendingAddSceneData;
			SceneDataRequestRemoved -= _runtimeLoadedSceneData.PendingRemoveSceneData;
		}

		void OnTriggerEnter(Collider collider)
		{
			if (IsSceneLoadingBoxLayer(collider.gameObject.layer))
			{
				return;
			}

			var sceneName = collider.gameObject.scene.name;

			if (_lastEnteredSceneName == sceneName)
			{
				return;
			}

			_lastEnteredSceneName = sceneName;
			SceneDataRequestAdded?.Invoke(gameObject, sceneName, _depthToLoad);
		}

		#endregion // UnityCallbacks

		public bool IsSceneLoadingBoxLayer(in int layer) => layer != _sceneLoadingBoxLayer;

		string _lastEnteredSceneName = string.Empty;
		[SerializeField] RuntimeLoadedSceneData _runtimeLoadedSceneData;
		[SerializeField] int _depthToLoad;
		readonly int _sceneLoadingBoxLayer = 6;
	}
}
using System;

using UnityEngine;

namespace MC
{
	/// <summary>
	/// 이 컴포넌트가 부착된 오브젝트가 씬의 경계 콜라이더에 접촉했을 특정 씬에 입장하였다는 이벤트를 발생시킨다.
	/// </summary>
	[DisallowMultipleComponent]
	public class SceneLoadTrigger : MonoBehaviour
	{
		/// <summary>
		/// 게임 오브젝트(GameObject)가 해당 이름을 가진 씬 이름에 접촉하였으며, 이 씬(string)으로부터 거리가 일정 이내(int)인 씬들이 로드되어야 함을 알리는 이벤트
		/// </summary>
		public event Action<GameObject, string, int> EnteredNewScene;

		#region UnityCallbacks

		void OnEnable()
		{
			EnteredNewScene += _runtimeLoadedSceneData.OnEnteredNewScene;
		}

		void OnDisable()
		{
			EnteredNewScene -= _runtimeLoadedSceneData.OnEnteredNewScene;
		}

		/// <summary>
		/// 이 스크립트가 부착된 오브젝트가 씬 바운드 콜라이더에 트리거 이벤트를 발생시켰을 때 콜백되어야 함
		/// </summary>
		/// <remarks>
		/// 한 씬 트리거를 여러번 연속해서 작동시킬 수 없으며, 레이어 마스크 설정을 제대로 확인할 필요가 있음.
		/// </remarks>
		void OnTriggerEnter(Collider sceneBoundCollider)
		{
			if (sceneBoundCollider.gameObject.layer != _sceneLoadingBoxLayer)
			{
				return;
			}

			var enteredSceneName = sceneBoundCollider.gameObject.scene.name;

			if (_lastEnteredSceneName == enteredSceneName)
			{

#if UNITY_EDITOR
				if (_logOnEnteringNewScene)
				{
					Debug.Log($"{gameObject} triggered {enteredSceneName}'s bound, but not activated event.");
				}
#endif

				return;
			}

#if UNITY_EDITOR
			if (_logOnEnteringNewScene)
			{
				Debug.Log($"{gameObject} Entered {enteredSceneName}");
			}
#endif

			_lastEnteredSceneName = enteredSceneName;

			EnteredNewScene?.Invoke(gameObject, enteredSceneName, _depthToLoad);
		}

		#endregion // UnityCallbacks

		string _lastEnteredSceneName = string.Empty;

		[SerializeField] RuntimeLoadedSceneData _runtimeLoadedSceneData;
		[SerializeField] int _depthToLoad;

		int _sceneLoadingBoxLayer = 6;

#if UNITY_EDITOR
		[SerializeField] bool _logOnEnteringNewScene = false;
#endif
	}
}
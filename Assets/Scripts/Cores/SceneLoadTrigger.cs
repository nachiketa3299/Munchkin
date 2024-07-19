using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
	/// <summary>
	/// 이 컴포넌트가 부착된 오브젝트가 씬의 경계 콜라이더에 접촉했을 때 이벤트를 발생시킨다.
	/// </summary>
	[DisallowMultipleComponent]
	public class SceneLoadTrigger : MonoBehaviour
	{
		public event Action<GameObject, string, int> EnteredNewScene;

		#region Unity Callbacks

		void OnEnable()
		{
			EnteredNewScene += _runtimeLoadedSceneData.OnEnteredNewScene;
		}

		/// <summary>
		/// 이 스크립트가 부착된 오브젝트가 씬 바운드 콜라이더에 트리거 이벤트를 발생시켰을 때 콜백되어야 함
		/// </summary>
		/// <remarks>
		/// 한 씬 트리거를 여러번 연속해서 작동시킬 수 없으며, 레이어 마스크 설정을 제대로 확인할 필요가 있음.
		/// </remarks>
		void OnTriggerEnter(Collider sceneBoundCollider)
		{
			var enteredSceneName = sceneBoundCollider.gameObject.scene.name;

			if (_lastEnteredSceneName == enteredSceneName)
			{
				return;
			}

			// 여기서부터 ㄹㅇ Entered 판정

			Debug.Log($"{gameObject} Entered {enteredSceneName}");

			_lastEnteredSceneName = enteredSceneName;
			EnteredNewScene?.Invoke(gameObject, enteredSceneName, _depthToLoad);
		}

		void OnDisable()
		{
			EnteredNewScene = null;
		}

		#endregion // Unity Callbacks

		string _lastEnteredSceneName = string.Empty;

		[SerializeField] RuntimeLoadedSceneData _runtimeLoadedSceneData;
		[SerializeField] int _depthToLoad;

#if UNITY_EDITOR
		// [SerializeField] bool _logOnEnteringNewScene = false;
#endif
	}
}
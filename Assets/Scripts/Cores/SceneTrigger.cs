using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
	/// <summary>
	/// 캐릭터가 씬의 콜라이더에 접촉했을 때 이벤트를 발생기키는 용도
	/// </summary>
	[DisallowMultipleComponent]
	public class SceneLoadTrigger : MonoBehaviour
	{

		#region Unity Callbacks

		/// <remarks>
		/// 한 씬 트리거를 여러번 연속해서 작동시킬 수 없음
		/// </remarks>
		void OnTriggerEnter(Collider other)
		{
			var enteredSceneName = other.gameObject.scene.name;

#if UNITY_EDITOR
			if (_logOnEnteringNewScene)
				Debug.Log($"{gameObject.name}가 씬 {other.gameObject.scene.name}의 {other.name}에 OnTriggerEnter 이벤트 수신");
#endif

			if (_lastEnteredSceneName == enteredSceneName)
			{

#if UNITY_EDITOR
				if (_logOnEnteringNewScene)
					Debug.Log($"씬 {enteredSceneName}에 대한 OnTriggerEnter는 바로 이전에 처리하였으므로, 무시합니다.");
#endif
				return;
			}


#if UNITY_EDITOR
			if (_logOnEnteringNewScene)
				Debug.Log($"씬 {enteredSceneName}에 대한 입장은 이번이 최초입니다. 이 씬을 기준으로 로딩 타겟을 결정합니다.");
#endif

			StartCoroutine(EnteredNewSceneRoutine(enteredSceneName));
		}

		#endregion // Unity Callbacks

		/// <summary>
		/// 씬 <paramref name="newSceneName"/>를 기준으로 거리가 <see cref="_depthToLoad"/> 내에 있는 씬들을 로드하고,
		/// <see cref="_depthToLoad"/>  밖에 있는 씬들을 언로드하는 코루틴
		/// </summary>
		IEnumerator EnteredNewSceneRoutine(string newSceneName)
		{
			_lastEnteredSceneName = newSceneName;

			var operations = new List<AsyncOperation>();
			foreach (var name in RetrieveLoadedGamePlaySceneNames())
			{
				if (_data.GetDistance(newSceneName, name) > _depthToLoad)
				{
					operations.Add(SceneManager.UnloadSceneAsync(name));
				}
			}

			foreach (var name in _data.RetrieveNearSceneNames(newSceneName, _depthToLoad))
			{
				if (SceneManager.GetSceneByName(name).isLoaded)
				{
					continue;
				}

				operations.Add(SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive));
			}

			while (operations.All(operation => operation?.isDone == true))
			{
				yield return null;
			}

			// load & unload finished
		}

		/// <summary>
		/// 현재 로드된 모든 씬 Persistent Scene을 제외한 모든 씬의 이름을 가져온다.
		/// </summary>
		List<string> RetrieveLoadedGamePlaySceneNames()
		{
			var sceneCounts = SceneManager.sceneCount;
			var nameList = new List<string>(sceneCounts); // allocate capacity

			for (var i = 0; i < sceneCounts; ++i)
			{
				var tName = SceneManager.GetSceneAt(i).name;

				if (tName == _data.PersistentSceneName)
				{
					continue;
				}

				nameList.Add(tName);
			}

			return nameList;
		}

		string _lastEnteredSceneName = String.Empty;
		[SerializeField] SceneDependencyData _data;
		[SerializeField] int _depthToLoad;

#if UNITY_EDITOR
		[SerializeField] bool _logOnEnteringNewScene = false;
#endif
	}
}
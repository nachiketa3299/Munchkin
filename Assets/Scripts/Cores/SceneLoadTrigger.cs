using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
	/// <summary>
	/// 오브젝트가 씬의 콜라이더에 접촉했을 때 이벤트를 발생기키는 용도
	/// </summary>
	/// <remarks>
	/// 하나의 씬 트리거는 여러번 연속해서 작동시킬 수 없음
	/// </remarks>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Collider))]
	public class SceneLoadTrigger : MonoBehaviour
	{
		void OnTriggerEnter(Collider other)
		{
			if (other.tag.Equals("MC_Pickable"))
				return;

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

		/// <summary>
		/// 씬 <paramref name="newSceneName"/>을 기준으로 거리가 <see cref="_depthToLoad"/> 내에 있는 씬들을 로드하고,
		/// <see cref="_depthToLoad"/>  밖에 있는 씬들을 언로드한다.
		/// </summary>
		IEnumerator EnteredNewSceneRoutine(string newSceneName)
		{
			_lastEnteredSceneName = newSceneName;

			var operations = new List<AsyncOperation>();
			foreach (var loadedSceneName in RetrieveLoadedGamePlaySceneNames())
			{
				if (_data.GetDistance(newSceneName, loadedSceneName) > _depthToLoad)
				{
					operations.Add(SceneManager.UnloadSceneAsync(loadedSceneName));
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

			// load / unload finished
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

		/*public void MoveObejct()
		{
			SceneManager.MoveGameObjectsToScene();
		}*/

		[SerializeField] SceneDependencyData _data;
		[SerializeField] int _depthToLoad;
		string _lastEnteredSceneName = String.Empty;

#if UNITY_EDITOR
		[SerializeField] bool _logOnEnteringNewScene = false;
#endif
	}
}
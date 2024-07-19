using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
	[DisallowMultipleComponent]
	public class SceneLoadManager : MonoBehaviour
	{

		#region Unity Callbacks

		void OnEnable()
		{
			_runtimeLoadedSceneData.AsyncLoadSceneOperationNeeded += OnAsyncLoadOperationNeeded;
			_runtimeLoadedSceneData.AsyncUnloadSceneOperationNeeded += OnAsyncUnloadOperationNeeded;
		}

		void OnDisable()
		{
			_runtimeLoadedSceneData.AsyncLoadSceneOperationNeeded -= OnAsyncLoadOperationNeeded;
			_runtimeLoadedSceneData.AsyncUnloadSceneOperationNeeded -= OnAsyncUnloadOperationNeeded;
		}

		#endregion // Unity Callbacks

		void OnAsyncLoadOperationNeeded(IEnumerable<string> allUniqueSceneNamesShouldBeLoaded)
		{
			var operations = new List<AsyncOperation>();

			foreach (var sceneName in allUniqueSceneNamesShouldBeLoaded)
			{
				if (SceneManager.GetSceneByName(sceneName).isLoaded)
				{
					continue;
				}

				operations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
			}

			StartCoroutine(AsyncLoadScenesRoutine(operations));
		}


		void OnAsyncUnloadOperationNeeded(IEnumerable<string> allUniqueSceneNamesShouldBeUnloaded)
		{
			var operations = new List<AsyncOperation>();

			foreach (var sceneName in allUniqueSceneNamesShouldBeUnloaded)
			{
				if (!SceneManager.GetSceneByName(sceneName).isLoaded)
				{
					continue;
				}

				operations.Add(SceneManager.UnloadSceneAsync(sceneName));
			}

			StartCoroutine(AsyncUnloadSceneRoutine(operations));
		}

		IEnumerator AsyncLoadScenesRoutine(IEnumerable<AsyncOperation> operations)
		{
			while (operations.All(operations => operations?.isDone == true))
			{
				yield return null;
			}
		}

		IEnumerator AsyncUnloadSceneRoutine(IEnumerable<AsyncOperation> operations)
		{
			while (operations.All(operations => operations?.isDone == true))
			{
				yield return null;
			}
		}

		[SerializeField] RuntimeLoadedSceneData _runtimeLoadedSceneData;
	}
}
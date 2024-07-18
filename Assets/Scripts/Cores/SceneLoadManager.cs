using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public class SceneLoadManager : MonoBehaviour
	{

		#region Unity Callbacks

		void Awake()
		{
		}

		void Start()
		{
			_dataManager.SceneOperationNeeded += (IEnumerable<AsyncOperation> operations) =>
			{
				StartCoroutine(SceneOperationRoutine(operations));
			};
		}

		#endregion // Unity Callbacks

		IEnumerator SceneOperationRoutine(IEnumerable<AsyncOperation> operations)
		{

			while (operations.All(operation => operation?.isDone == true))
			{
				yield return null;
			}
		}

		[SerializeField] RuntimeLoadedSceneData _dataManager;
	}
}
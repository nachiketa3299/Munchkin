using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MC
{
	[DisallowMultipleComponent]
	public partial class GrabAction : ActionRoutineBase
	{

		#region Unity Callbacks

		void Awake()
		{

#if UNITY_EDITOR
			if (!_grabSocket)
			{
				Debug.LogWarning("grabSocket에 아무것도 할당되지 않았습니다.");
			}
#endif
		}

		#endregion // Unity Callbacks

		public void BeginAction()
		{

#if UNITY_EDITOR
			Debug.Log("Grab Action Begin");
#endif
			TryStopCurrentRoutine();

			if (_grabbing)
			{
				return;
			}

			_currentRoutine = StartCoroutine(FindGrabObjectRoutine());
		}

		IEnumerator FindGrabObjectRoutine()
		{
			var resultCount = Physics.OverlapSphereNonAlloc(transform.position, 3.0f, _overlapResults, _grabObjectMask);

			if (resultCount <= 0)
			{

#if UNITY_EDITOR
				Debug.Log("주변에 집을 만한 것이 전혀 없다.");
#endif

				yield break;
			}

			var uniqueGrabObjects = new HashSet<GameObject>(); // 컴파운드 콜라이더를 가진 게임 오브젝트일 가능성이 있음

			for (var i = 0; i < resultCount; ++i)
			{
				uniqueGrabObjects.Add(_overlapResults[i].gameObject);
			}

			// 추가적인 Find 로직을 여기서 구현

			_grabObject = uniqueGrabObjects
				.OrderBy(obj => (obj.transform.position - transform.position).sqrMagnitude)
				.FirstOrDefault()?
				.GetComponent<GrabObject>();

			if (!_grabObject)
			{
#if UNITY_EDITOR
				Debug.Log("주변에 GrabObject 컴포넌트를 가진 잡을 만한 것이 없다.");
#endif
			}

			_currentRoutine = StartCoroutine(GrabRoutine());
		}

		IEnumerator GrabRoutine()
		{
			_grabObject.BeginGrabState(_grabSocket);

			yield break;
		}

		public void EndAction()
		{

			// _grabObject?.EndGrabState();

#if UNITY_EDITOR
			Debug.Log("Grab Action End");
#endif
			// Actually, there is nothing to do in this method.

		}

		Collider[] _overlapResults = new Collider[5];
		GrabObject _grabObject;
		bool _grabbing = false;

		[SerializeField] LayerMask _grabObjectMask = 1 << 8;
		[SerializeField] GameObject _grabSocket;

#if UNITY_EDITOR
		bool _logOnGrabAction = false;
#endif
	}
}
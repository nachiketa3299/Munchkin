using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public partial class GrabThrowAction : ActionRoutineBase
	{

		#region UnityCallbacks

		void Awake()
		{
			// Cache components

			_rigidbody = GetComponent<Rigidbody>();
		}

		#endregion // UnityCallbacks

		public void BeginAction(in float directionValue)
		{
			TryStopCurrentRoutine();

			if (!_isGrabbing)
			{
				_currentRoutine = StartCoroutine(GrabRoutine());
			}
			else
			{
				_currentRoutine = StartCoroutine(ThrowRoutine(directionValue));
			}

		}

		IEnumerator GrabRoutine()
		{
			var resultCount = Physics.OverlapSphereNonAlloc
			(
				position: transform.position,
				radius: 3.0f,
				results: _overlapResults,
				layerMask: _grabThrowObjectMask
			);

			if (resultCount <= 0)
			{
				yield break;
			}

			var uniqueGrabObjects = new HashSet<GameObject>(); // 컴파운드 콜라이더를 가진 게임 오브젝트일 가능성이 있음

			for (var i = 0; i < resultCount; ++i)
			{
				uniqueGrabObjects.Add(_overlapResults[i].gameObject);
			}

			// 추가적인 Find 로직이 있다면 여기서 구현

			_grabThrowObject = uniqueGrabObjects
				.OrderBy(obj => (obj.transform.position - transform.position).sqrMagnitude)
				.FirstOrDefault()?
				.transform.root.gameObject
				.GetComponent<GrabThrowTarget>();

			_grabThrowObject.BeginGrabState(_grabThrowSocket);
			_isGrabbing = true;

			yield break;
		}

		IEnumerator ThrowRoutine(float directionValue)
		{
			_grabThrowObject.EndGrabState();
			_isGrabbing = false;

			// Vertical Throwing
			if (directionValue == 1.0f)
			{
				var throwDirection = new Vector3(0.0f, directionValue, 0.0f);
				_grabThrowObject.Throw(_rigidbody.velocity, throwDirection * _throwForceVertical);
			}
			// Horizontal Throwing
			else
			{
				var throwDirection = transform.forward;
				_grabThrowObject.Throw(_rigidbody.velocity, throwDirection * _throwForceHorizontal);
			}

			_grabThrowObject = null;

			yield break;
		}

		Rigidbody _rigidbody;
		Collider[] _overlapResults = new Collider[5];
		GrabThrowTarget _grabThrowObject;
		bool _isGrabbing = false;

		[SerializeField] LayerMask _grabThrowObjectMask = 1 << 8;
		[SerializeField] GameObject _grabThrowSocket;
		[SerializeField] float _throwForceHorizontal = 300.0f;
		[SerializeField] float _throwForceVertical = 600.0f;
	}
}
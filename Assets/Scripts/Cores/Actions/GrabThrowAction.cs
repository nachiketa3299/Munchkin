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
				results: _overlapResultCache,
				layerMask: _grabThrowObjectMask
			);

			if (resultCount <= 0)
			{
				yield break;
			}

			// 콜라이더를 소유한 유일한 오브젝트에 대한 HashSet 형성

			var uniqueGrabbableObjects = new HashSet<GameObject>();

			for (var i = 0; i < resultCount; ++i)
			{
				uniqueGrabbableObjects.Add(_overlapResultCache[i].gameObject);
			}

			// 추가적인 Find 로직이 있다면 여기서 구현

			_grabThrowTarget = uniqueGrabbableObjects
				.OrderBy(obj => (obj.transform.position - transform.position).sqrMagnitude)
				.FirstOrDefault()?
				.transform.root.gameObject
				.GetComponent<GrabThrowTarget>();

			_grabThrowTarget.BeginGrabState(_grabThrowSocket);
			_isGrabbing = true;

			yield break;
		}

		IEnumerator ThrowRoutine(float directionValue)
		{
			_grabThrowTarget.EndGrabState();
			_isGrabbing = false;

			// Vertical Throwing
			if (directionValue == 1.0f)
			{
				var throwDirection = transform.up * directionValue;
				_grabThrowTarget.Throw
				(
					lastThrowerVelocity: _rigidbody.velocity,
					force: throwDirection * _throwForceVertical
				);
			}
			// Horizontal Throwing
			else
			{
				var throwDirection = transform.forward;
				_grabThrowTarget.Throw
				(
					lastThrowerVelocity: _rigidbody.velocity,
					force: throwDirection * _throwForceHorizontal
				);
			}

			_grabThrowTarget = null;

			yield break;
		}

		Rigidbody _rigidbody;
		Collider[] _overlapResultCache = new Collider[5];
		GrabThrowTarget _grabThrowTarget;
		bool _isGrabbing = false;

		[SerializeField] LayerMask _grabThrowObjectMask = 1 << 8;
		[SerializeField] GameObject _grabThrowSocket;
		[SerializeField] float _throwForceHorizontal = 300.0f;
		[SerializeField] float _throwForceVertical = 600.0f;
	}
}
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

	/// <summary>
	/// 아무것도 잡고 있지 않다면, 무언가 잡으려고 시도한다. <br/>
	/// 이미 잡고 있다면, <paramref name="directionValue"/> 를 이용해 던진다.
	/// </summary>
	public void BeginAction(in float directionValue)
	{
		TryStopCurrentRoutine();

		_currentRoutine = !_grabThrowTarget
			? StartCoroutine(GrabRoutine())
			: StartCoroutine(ThrowRoutine(directionValue));
	}

	public void EndAction()
	{
		if (!_grabThrowTarget)
		{
			return;
		}

		TryDetachTargetFromSocket();
		_grabThrowTarget.EndGrabState();
		_grabThrowTarget = null;
	}

	IEnumerator GrabRoutine()
	{
		if (!(_grabThrowTarget = FindGrabTarget()))
		{
			yield break;
		}

		_grabThrowTarget.BeginGrabState();

		TryAttachTargetToSocket();

		_grabThrowTarget.GrabThrowTargetDisabled += EndAction;

		yield break;
	}

	IEnumerator ThrowRoutine(float directionValue)
	{

		TryDetachTargetFromSocket();

		_grabThrowTarget.EndGrabState();

		_grabThrowTarget.AddForce(CalculateThrowForce(directionValue));

		_grabThrowTarget.GrabThrowTargetDisabled -= EndAction;

		_grabThrowTarget = null;

		yield break;
	}
	GrabThrowTarget FindGrabTarget()
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
			return null;
		}

		var uniqueTargets = new HashSet<GameObject>();

		for (var i = 0; i < resultCount; ++i)
		{
			uniqueTargets.Add(_overlapResultCache[i].gameObject);
		}

		var optimalTarget = uniqueTargets
			.OrderBy(obj => (obj.transform.position - transform.position).sqrMagnitude)
			.FirstOrDefault()?
			.transform.root.gameObject
			.GetComponent<GrabThrowTarget>();

		return optimalTarget;
	}

	void TryAttachTargetToSocket()
	{
		if (!_grabThrowTarget)
		{
			Debug.Log("Try attaching, but failed.");
			return;
		}

		// only process if there is target

		// set position
		_grabThrowTarget.transform.SetPositionAndRotation
		(
			position: _grabThrowSocket.transform.position,
			rotation: Quaternion.identity
		);

		// set parent
		_grabThrowTarget.transform.SetParent(_grabThrowSocket.transform);
		_grabThrowTarget.AddForce(_rigidbody.velocity);
	}

	void TryDetachTargetFromSocket()
	{
		if (!_grabThrowTarget)
		{
			return;
		}

		// only process if there is target

		// unset parent
		_grabThrowTarget.transform.SetParent(null);
	}

	Vector3 CalculateThrowForce(in float directionValue)
	{
		var throwForce = directionValue == 1.0f
			? transform.up * directionValue * _throwForceVertical
			: transform.forward * _throwForceHorizontal
			+ _rigidbody.velocity;

		return throwForce;
	}

	Rigidbody _rigidbody;
	Collider[] _overlapResultCache = new Collider[5];
	GrabThrowTarget _grabThrowTarget;
	[SerializeField] LayerMask _grabThrowObjectMask = 1 << 8;
	[SerializeField] GameObject _grabThrowSocket;
	[SerializeField] float _throwForceHorizontal = 10.0f;
	[SerializeField] float _throwForceVertical = 10.0f;
}

}
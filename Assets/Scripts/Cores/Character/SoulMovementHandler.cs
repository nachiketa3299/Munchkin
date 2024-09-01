using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LifespanHandler))]
public partial class SoulMovementHandler : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();

		_lifespanHandler = GetComponent<LifespanHandler>();

		// Bind events

		_lifespanHandler.Ended += StartSoulMovementState;
	}

	void Start()
	{
		EggPool.Instance.InstanceEnabled += OnEggInstanceEnabled;
		EggPool.Instance.InstanceDisabled += OnEggInstanceDisabled;
	}

	void OnDestroy()
	{
		// Unbind events

		_lifespanHandler.Ended -= StartSoulMovementState;

		EggPool.Instance.InstanceEnabled -= OnEggInstanceEnabled;
		EggPool.Instance.InstanceDisabled -= OnEggInstanceDisabled;
	}

#endregion // UnityCallbacks

	void OnEggInstanceEnabled(EggLifecycleHandler _)
	{
		if (_isInSoulState)
		{
			FindOptimalEgg();
		}
	}

	void OnEggInstanceDisabled()
	{
		if (_isInSoulState)
		{
			FindOptimalEgg();
		}
	}

#region UnityCollision

// Egg 에 닿은 경우, Egg 를 소멸시키고, 생애주기를 다시 시작해야함
void OnTriggerEnter(Collider collider)
{
	if (collider.gameObject.layer != _eggNormalLayer)
	{
		return;
	}

	_reachedDestination = true;
	StopAllCoroutines();
	EndSoulMovementState();

	var collidedEgg = collider.transform.root.GetComponent<EggLifecycleHandler>();
	collidedEgg.EndLifecycle();
}

#endregion // UnityCollision

void StartSoulMovementState()
{
	_isInSoulState = true;

	_reachedDestination = false;

	// Rigidbody settings
	_rigidbody.useGravity = false;
	_rigidbody.isKinematic = true;

	// Find Optimal egg
	_optimalEgg = FindOptimalEgg();

	StartCoroutine(SoulMovementRoutine());
}

void EndSoulMovementState()
{
	_isInSoulState = false;

	// Rigidbody settings
	_rigidbody.useGravity = true;
	_rigidbody.isKinematic = false;

	_lifespanHandler.RestartLifespan();
}

/// <summary>
///  반드시 성공해야함
/// </summary>
EggLifecycleHandler FindOptimalEgg()
{
	EggLifecycleHandler optimalEgg = null;

	// 1. 캐릭터 알이 있다면, 그 알 중 가장 캐릭터와 거리가 가까운 것

	if (EggPool.Instance.CharacterEggs.Count != 0)
	{
		optimalEgg = FindClosestEgg(EggPool.Instance.CharacterEggs);
	}

	// 2. 캐릭터 알이 없다면, 둥지 알 중 가장 캐릭터와 거리가 가까운 것

	else if (EggPool.Instance.NestEggs.Count != 0)
	{
		optimalEgg = FindClosestEgg(EggPool.Instance.NestEggs);
	}

	// 캐릭터 알과 둥지 알이 어떤 이유에서든 없다면, 둥지의 스폰 포지션으로
	else
	{
		Debug.LogError("Something went truly wrong...");
	}

	return optimalEgg;
}

EggLifecycleHandler FindClosestEgg(IList<EggLifecycleHandler> eggList) => eggList.OrderBy(egg => (egg.transform.position - transform.position).sqrMagnitude).FirstOrDefault();

IEnumerator SoulMovementRoutine()
{
	while (!_reachedDestination)
	{
		if (!_optimalEgg.isActiveAndEnabled)
		{
			_optimalEgg = FindOptimalEgg();
		}

		var newPosition = Vector3.Lerp(_rigidbody.position, _optimalEgg.transform.position, _moveSpeed * Time.fixedDeltaTime);
		_rigidbody.MovePosition(newPosition);

		yield return new WaitForFixedUpdate();
	}
}

Rigidbody _rigidbody;
LifespanHandler _lifespanHandler;

EggLifecycleHandler _optimalEgg = null;
[SerializeField] float _moveSpeed = 3f;

bool _isInSoulState = false;

bool _reachedDestination = false;
readonly int _eggNormalLayer = 8;

}

}
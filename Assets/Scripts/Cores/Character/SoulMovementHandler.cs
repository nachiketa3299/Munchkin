using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LifespanHandler))]
[RequireComponent(typeof(EggAction))]
public class SoulMovementHandler : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();

		_lifespanHandler = GetComponent<LifespanHandler>();
		_eggAction = GetComponent<EggAction>();

#if UNITY_EDITOR
		if (!_nestEggHandler)
		{
			Debug.LogWarning("Nest Egg Handler가 할당되지 않았습니다.");
		}
#endif

		// Bind events

		_lifespanHandler.Started += OnLifespanStarted;
		_lifespanHandler.Ended += OnLifespanEnded;
	}

	void OnDestroy()
	{
		// Unbind events

		_lifespanHandler.Started -= OnLifespanStarted;
		_lifespanHandler.Ended -= OnLifespanEnded;
	}

#endregion // UnityCallbacks

#region UnityCollision

// Egg 에 닿은 경우, Egg 를 소멸시키고, 생애주기를 다시 시작해야함
void OnTriggerEnter(Collider collider)
{
	var rootGo = collider.transform.root.gameObject;

	// COLLISION GUARD STARTS
	if (_collidingObjects.Contains(rootGo))
	{
		return;
	}

	_collidingObjects.Add(rootGo);

	// Here logics //

	// 만일 Egg이고, OptimalEgg인 경우,
	if (rootGo.TryGetComponent<EggLifecycleHandler>(out var incomingEgg) && incomingEgg.Equals(_optimalEgg))
	{
		StopAllCoroutines();
		EndSoulState();

		incomingEgg.EndLifecycle();
	}

	// COLLISION GUARD ENDS
	StartCoroutine(Reset(rootGo));

}

IEnumerator Reset(GameObject collidingObject)
{
	yield return new WaitForEndOfFrame();
	_collidingObjects.Remove(collidingObject);
}

#endregion // UnityCollision

void OnLifespanStarted()
{
	EndSoulState();
}

void OnLifespanEnded()
{
	BeginSoulState();
}

void TryFindOptimalEgg()
{
	// 여기에 달걀을 찾는 로직을 작성해야함
	// 아마도 여기에다가 작성하는 것이 옳을 것 같음
}

void BeginSoulState()
{
	_rigidbody.useGravity = false;
	_rigidbody.isKinematic = true;

	StartCoroutine(SoulMovementRoutine());
}

void EndSoulState()
{
	_rigidbody.useGravity = true;
	_rigidbody.isKinematic = false;

	_lifespanHandler.RestartLifespan();
}

IEnumerator SoulMovementRoutine()
{
	// 이거 그냥 모션을 움직이는 것으로
	while (!_reachedDestination)
	{
		_rigidbody.AddForce(transform.forward * _moveSpeed, ForceMode.VelocityChange);

		_spawnPosition = _optimalEgg != null ? _optimalEgg.transform.position : _nestEggHandler.SpawnPosition;

		var direction = _spawnPosition - transform.position;
		var rotation = Quaternion.LookRotation(direction);

		_rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, _turnSpeed));

		yield return new WaitForFixedUpdate();
	}
}

Rigidbody _rigidbody;
LifespanHandler _lifespanHandler;
Vector3 _spawnPosition;

[SerializeField] NestEggHandler _nestEggHandler;
EggAction _eggAction;

EggLifecycleHandler _optimalEgg = null;

List<EggLifecycleHandler> _nestEggs;
List<EggLifecycleHandler> _characterEggs;

HashSet<GameObject> _collidingObjects = new();
bool _reachedDestination = false;
[SerializeField] float _moveSpeed = 10f;
[SerializeField] float _turnSpeed = 10f;

}

}
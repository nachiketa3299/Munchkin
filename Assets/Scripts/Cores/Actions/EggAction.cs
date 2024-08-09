using System.Collections;
using System.Linq;

using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EggFactory))]
public partial class EggAction : ActionRoutineBase
{

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();
		_eggFactory = GetComponent<EggFactory>();
	}

#endregion // UnityCallbacks

	public void BeginAction()
	{
		TryStopCurrentRoutine();

		if (_alreadyLayed)
		{
			return;
		}

		_currentRoutine = StartCoroutine(LayEggChargeRoutine());
	}

	public void EndAction()
	{
		TryStopCurrentRoutine();

		_eggActionChargeTimeCurrent = 0.0f;
	}

	IEnumerator LayEggChargeRoutine()
	{
		_eggActionChargeTimeCurrent = 0.0f;

		while (Mathf.Clamp01(_eggActionChargeTimeCurrent / _eggActionChargeTimeMax) < 1.0f)
		{
			_eggActionChargeTimeCurrent += Time.deltaTime;
			yield return null;
		}

		PerformEggAction();

		// Perform egg action recoil
		_rigidbody.AddForce(transform.up * 10.0f, ForceMode.Impulse);

		_eggActionChargeTimeCurrent = 0.0f;
		_currentRoutine = null;
	}

	void PerformEggAction()
	{
		var spawnPosition = FindSpawnPosition();
		var spawnedInstance = _eggFactory.TakeInitializedEggFromPool(owner: EEggOwner.Character);
		spawnedInstance.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

		// 이 시점 이후에서야 알이 활성화되고, 위치가 특정된다.

		// 만일 생성 시점에 알이 캐릭터와 클리핑하고 있으면,
		// 더 이상 클리핑 하지 않을 때까지 캐릭터와의 충돌을 무시한다.

		if (!IsOverlapping(spawnedInstance))
		{
			return;
		}

		SetLayerRecursive(spawnedInstance, _eggPreparingLayer);
		StartCoroutine(CheckIsOverlappingRoutine(spawnedInstance));
	}

	bool IsOverlapping(GameObject target)
	{
		var targetPhysicalColliders = GetComponentsInChildren<Collider>()
			.Where(collider => !collider.isTrigger)
			.ToArray();

		var targetBounds = targetPhysicalColliders[0].bounds;
		for (var i = 1; i < targetPhysicalColliders.Length; ++i)
		{
			targetBounds.Encapsulate(targetPhysicalColliders[i].bounds);
		}

		return Physics.CheckBox
		(
			center: target.transform.position,
			halfExtents: targetBounds.extents,
			orientation: target.transform.rotation,
			layerMask: 1 << gameObject.layer
		);
	}

	IEnumerator CheckIsOverlappingRoutine(GameObject target)
	{
		while (IsOverlapping(target))
		{
			yield return new WaitForFixedUpdate();
		}

		SetLayerRecursive(target, _eggNormalLayer);
	}

	static void SetLayerRecursive(GameObject target, in int newLayer)
	{
		target.layer = newLayer;

		foreach(Transform childTransform in target.transform)
		{
			SetLayerRecursive(childTransform.gameObject, newLayer);
		}
	}

	Vector3 FindSpawnPosition()
	{
		var spawnPosition = transform.position;
		var isThereObstacle = Physics.Raycast
		(
			origin: spawnPosition,
			direction: -1.0f * transform.up,
			hitInfo: out var hitInfo,
			maxDistance: _rayCastMaxDistance,
			layerMask: gameObject.layer
		);

		if (isThereObstacle)
		{
			spawnPosition.y += _eggPhysicalData.CombinedPhysicalBounds.extents.y;
		}

		return spawnPosition;
	}

	Rigidbody _rigidbody;
	bool _alreadyLayed = false;
	[HideInInspector] [SerializeField] float _eggActionChargeTimeCurrent = 0.0f;
	[SerializeField] float _eggActionChargeTimeMax = 1.0f;
	float _rayCastMaxDistance = 10.0f;
	[SerializeField] EggPhysicalData _eggPhysicalData;
	EggFactory _eggFactory;
	int _eggPreparingLayer = 9;
	int _eggNormalLayer = 8;
}

}
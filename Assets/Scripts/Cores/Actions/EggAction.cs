using System.Collections;
using System.Linq;

using UnityEngine;

using MC.Utility;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public partial class EggAction : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();

// #if UNITY_EDITOR
// 		if (_eggPool == null)
// 		{
// 			Debug.Log("EggAction에 RuntimePooledEggData가 설정되지 않았습니다.");
// 		}
// #endif

	}

#endregion // UnityCallbacks

	public void BeginAction()
	{
		if (_eggActionRoutine != null)
		{
			StopCoroutine(_eggActionRoutine);
		}

		if (_alreadyLayed)
		{
			return;
		}

		_eggActionRoutine = StartCoroutine(EggActionChargeRoutine());
	}

	public void EndAction()
	{
		if (_eggActionRoutine != null)
		{
			StopCoroutine(_eggActionRoutine);
		}

		_eggActionChargeTimeCurrent = 0.0f;
	}

	IEnumerator EggActionChargeRoutine()
	{
		_eggActionChargeTimeCurrent = 0.0f;

		while (Mathf.Clamp01(_eggActionChargeTimeCurrent / _eggActionChargeTimeMax) < 1.0f)
		{
			_eggActionChargeTimeCurrent += Time.deltaTime;
			yield return null;
		}

		PerformEggAction();

		// Perform egg action recoil
		_rigidbody.AddForce(transform.up * _eggActionRecoil, ForceMode.Impulse);

		_eggActionChargeTimeCurrent = 0.0f;

		_eggActionRoutine = null;
	}

	void PerformEggAction()
	{
		var spawnPosition = FindEggSpawnPosition();
		var layedEgg = EggPool.Instance.GetEggInstance(EEggOwner.Character);
		layedEgg.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

		// 이 시점 이후에서야 알이 활성화되고, 위치가 특정된다.
		// 만일 생성 시점에 알이 캐릭터와 클리핑하고 있으면
		// 더 이상 클리핑 하지 않을 때까지 알과 캐릭터의 충돌을 무시한다.

		if (!IsOverlappingWithCharacter(layedEgg))
		{
			return;
		}

		LayerUtility.SetLayerRecursive(layedEgg.gameObject, _eggPreparingLayer);

		// 아래는 다수의 Egg에 대해서 알아서 작동해야 하므로 코루틴을 캐싱할 필요가 없다. (각 Egg마다 코루틴 생성)
		StartCoroutine(CheckIsOverlappingRoutine(layedEgg));
	}

	// I can't remember wtf this means but now ..., I don't even care
	bool IsOverlappingWithCharacter(EggLifecycleHandler layedEgg)
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
			center: layedEgg.transform.position,
			halfExtents: targetBounds.extents,
			orientation: layedEgg.transform.rotation,
			layerMask: 1 << gameObject.layer
		);
	}

	IEnumerator CheckIsOverlappingRoutine(EggLifecycleHandler layedEgg)
	{
		while (IsOverlappingWithCharacter(layedEgg))
		{
			yield return new WaitForFixedUpdate();
		}

		LayerUtility.SetLayerRecursive(layedEgg.gameObject, _eggNormalLayer);
	}

	/// <summary>
	/// EggAction을 수행하는 오브젝트와 Egg의 크기를 기준으로 Egg가 지형과 클리핑되지 않는 소환 위치를 찾는다.
	/// </summary>
	Vector3 FindEggSpawnPosition()
	{
		// TODO 이 게임에 *경사*라는 개념이 생겼을 때, 반드시 다시 테스트해야 한다.
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
	Coroutine _eggActionRoutine;
	bool _alreadyLayed = false;
	[SerializeField][HideInInspector] float _eggActionChargeTimeCurrent = 0.0f;
	[SerializeField] float _eggActionChargeTimeMax = 1.0f;
	[SerializeField] float _eggActionRecoil = 10.0f;
	float _rayCastMaxDistance = 10.0f;
	[SerializeField] EggPhysicalData _eggPhysicalData;
	readonly int _eggPreparingLayer = 9;
	readonly int _eggNormalLayer = 8;
	// [SerializeField] EggPoolManager _eggPool;
}

}
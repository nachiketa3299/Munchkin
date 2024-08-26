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
}

}
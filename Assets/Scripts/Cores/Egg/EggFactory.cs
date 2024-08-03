using System.Linq;
using System.Collections;

using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public class EggFactory : MonoBehaviour
{
	#region UnityCallbacks

	void Awake()
	{
#if UNITY_EDITOR
		if (!_eggPool)
		{
			Debug.LogWarning("RuntimePooledEggData 가 설정되지 않았습니다.");
		}
#endif
	}

	#endregion // UnityCallbacks

	public void GetEggByNest()
	{

	}

	/// <summary>
	/// <paramref name="characterObject"/>를 기준으로 알을 스폰할 위치를 탐색하고, 해당 위치에 알을 스폰한다. <br/>
	/// </summary>
	/// <remarks>
	/// 만일 스폰 시점에 <paramref name="characterObject"/>와 알이 겹친다면, 더이상 겹치지 않을 때까지 레이어를 관리한다.
	/// </remarks>
	public void GetEggByCharacter(GameObject characterObject, in float raycastDistance)
	{
		var spawnPosition = FindSpawnPositionByRaycast
		(
			pivotGameObject: characterObject,
			rayCastDistance: raycastDistance,
			maskToIgnore: characterObject.layer
		);

		var spawnedObject = _eggPool.CharacterEggPool.Get().gameObject;
		spawnedObject.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

		// 이 시점 이후에서야 알이 활성화되고, 위치가 특정됨

		if (!IsOverlapping(creator: characterObject, creation: spawnedObject))
		{
			return;
		}

		SetLayerRecursive(spawnedObject, _eggPreparingLayer);
		StartCoroutine(CheckIsOverlappingRoutine(creator: characterObject, creation: spawnedObject));
	}

	static void SetLayerRecursive(GameObject gameObject, in int newLayer)
	{
		gameObject.layer = newLayer;

		foreach(Transform childTransform in gameObject.transform)
		{
			SetLayerRecursive(childTransform.gameObject, newLayer);
		}
	}

	/// <summary>
	/// 매 고정 프레임마다, <paramref name="creator"/>와 <paramref name="creation"/>이 겹치는지 확인한다.
	/// </summary>
	IEnumerator CheckIsOverlappingRoutine(GameObject creator, GameObject creation)
	{
		while (IsOverlapping(creator, creation))
		{
			yield return new WaitForFixedUpdate();
		}

		SetLayerRecursive(creation, _eggNormalLayer);
	}

	bool IsOverlapping(GameObject creator, GameObject creation)
	{
		var creatorLayer = creator.layer;
		var creationPhysicalColliders = GetComponentsInChildren<Collider>() // 퍼포먼슈 이슈는 일단 생각하지 마
			.Where(collider => !collider.isTrigger)
			.ToArray();

		var creationBounds = creationPhysicalColliders[0].bounds;
		for (var i = 1; i < creationPhysicalColliders.Length; ++i)
		{
			creationBounds.Encapsulate(creationPhysicalColliders[i].bounds);
		}

		return Physics.CheckBox
		(
			center: creation.transform.position,
			halfExtents: creationBounds.extents,
			orientation: creation.transform.rotation,
			layerMask: 1 << creatorLayer
		);
	}
	public void ReturnEgg(EggLifecycleHandler egg)
	{
		_eggPool.CharacterEggPool.Release(egg);
		StopAllCoroutines();
	}

	Vector3 FindSpawnPositionByRaycast(GameObject pivotGameObject, in float rayCastDistance, in int maskToIgnore)
	{
		var spawnPosition = pivotGameObject.transform.position;

		var isThereObstacle = Physics.Raycast
		(
			origin: spawnPosition,
			direction: -1.0f * pivotGameObject.transform.up,
			hitInfo: out var hitInfo,
			maxDistance: rayCastDistance,
			layerMask: maskToIgnore
		);

		if (isThereObstacle)
		{
			spawnPosition.y += _eggPhysicalData.CombinedPhysicalBounds.extents.y;
		}

		return spawnPosition;
	}

	[SerializeField] RuntimePooledEggData _eggPool;
	[SerializeField] EggPhysicalData _eggPhysicalData;

	int _eggPreparingLayer = 9;
	int _eggNormalLayer = 8;
}

}
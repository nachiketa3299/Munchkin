using System.Linq;

using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public partial class EggFactory : MonoBehaviour
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

		void OnTriggerExit(Collider collider)
		{
			if (collider.gameObject.layer != _eggLayerMaskPreparing)
			{
				return;
			}

			var triggerColliderChildObject = collider.gameObject;
			triggerColliderChildObject.SetActive(false);

			SetEggLayerMaskRecursively
			(
				root: collider.gameObject.transform.root.gameObject,
				newLayer: _eggLayerMaskNormal
			);
		}

		#endregion // UnityCallbacks

		/// <summary>
		/// 캐릭터 오브젝트를 기준으로 알을 스폰할 위치를 탐색하고, 해당 위치에 알을 스폰한다.
		/// </summary>
		public void GetEggByCharacter(GameObject characterObject, in float raycastDistance)
		{
			var spawnPosition = FindSpawnPositionByRaycast
			(
				pivotObject: characterObject,
				rayCastDistance: raycastDistance,
				maskToIgnore: characterObject.layer
			);

			var spawnedObject = _eggPool.Pool.Get().gameObject;
			spawnedObject.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

			// TODO 이게 null 일 경우, 정확히는 Egg의 Object Hierarchy가 에디터에서 제대로 설정되지 않은 경우의 처리에 대해 생각
			var triggerColliderChildObject = spawnedObject.GetComponentsInChildren<Collider>(includeInactive: true)
				.Where(collider => collider.isTrigger)
				.FirstOrDefault()?
				.gameObject;

			triggerColliderChildObject.SetActive(true);

			SetEggLayerMaskRecursively(spawnedObject, _eggLayerMaskPreparing);
		}

		public void ReturnEgg(EggLifecycleHandler egg)
		{
			_eggPool.Pool.Release(egg);
		}

		static void SetEggLayerMaskRecursively(GameObject root, in int newLayer)
		{
			root.layer = newLayer;
			foreach (Transform childTransform in root.transform)
			{
				SetEggLayerMaskRecursively(childTransform.gameObject, newLayer);
			}
		}

		Vector3 FindSpawnPositionByRaycast(GameObject pivotObject, in float rayCastDistance, in int maskToIgnore)
		{
			var spawnPosition = pivotObject.transform.position;

			var isThereObstacle = Physics.Raycast
			(
				origin: spawnPosition,
				direction: -1.0f * pivotObject.transform.up,
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

		int _eggLayerMaskNormal = 8;
		int _eggLayerMaskPreparing = 9;
	}
}
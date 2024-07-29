using System.Collections;

using UnityEngine;

namespace MC
{

	[DisallowMultipleComponent]
	[RequireComponent(typeof(EggFactory))]
	public partial class EggAction : ActionRoutineBase
	{
		#region Unity Callbacks

		void Awake()
		{
			_eggFactory = GetComponent<EggFactory>();

#if UNITY_EDITOR
			if (!_eggFactory)
			{
				Debug.LogWarning("EggFactory 컴포넌트를 찾을 수 없습니다.");
			}
#endif
		}

		void OnTriggerEnter(Collider collider)
		{
			Debug.Log($"OnTriggerEnter Called: target mask is {collider.gameObject.layer}");

			// Preparing 마스크만 .. 허용

			if ((1 << collider.gameObject.layer & 1 << _eggLayerMaskPreparing) == 0)
			{
				return;
			}

			Debug.Log($"{gameObject.name} trigger enters {collider.gameObject.name}");
		}

		void OnTriggerExit(Collider collider)
		{
			Debug.Log($"OnTriggerExit Called: target mask is {collider.gameObject.layer}");

			// Preparing 마스크만 허용


			if ((1 << collider.gameObject.layer & 1 << _eggLayerMaskPreparing) == 0)
			{
				return;
			}

			Debug.Log($"{gameObject.name} trigger exits {collider.gameObject.name}");

			SetEggLayerMaskRecursively(collider.gameObject.transform.root.gameObject, _eggLayerMaskNormal);
			collider.gameObject.SetActive(false);

			Debug.Log($"{collider.gameObject} layer mask set to {LayerMask.LayerToName(gameObject.layer)}");
		}

		#endregion

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

			_layChargeElapsedTime = 0.0f;

			if (_alreadyLayed)
			{
				return;
			}

			// PerformLayEgg();
		}

		IEnumerator LayEggChargeRoutine()
		{
			_layChargeElapsedTime = 0.0f;

			while (CurrentLayChargeRatio < 1.0f)
			{
				_layChargeElapsedTime += Time.deltaTime;
				yield return null;
			}

			PerformLayAction();

			_layChargeElapsedTime = 0.0f;
			_currentRoutine = null;
		}

		void PerformLayAction()
		{
			// Find place for spawning egg

			var isThereObstacle = Physics.Raycast
			(
				origin: transform.position,
				direction: -1.0f * transform.up,
				hitInfo: out var hitInfo,
				maxDistance: _rayCastMaxDistance,
				layerMask: gameObject.layer // 액션을 수행하는 자의 layerMask 는 무시
			);

			// Instancing Egg

			// var instance = Instantiate(_eggPrefab, transform.position, Quaternion.identity);
			var instance = _eggFactory.Spawn(transform.position, Quaternion.identity, true).gameObject;
			SetEggLayerMaskRecursively(instance, _eggLayerMaskPreparing);

			// Placing Egg

			_layPosition = transform.position;

			if (isThereObstacle)
			{
				_layPosition.y += instance.GetComponent<EggBounds>().CombinedBounds.extents.y;
			}

			instance.transform.position = _layPosition;

			// Lay egg action recoil

			GetComponent<Rigidbody>()?.AddForce(transform.up * 10.0f, ForceMode.Impulse);

			// _alreadyLayedEgg = true;
		}

		static void SetEggLayerMaskRecursively(GameObject rootGameObject, int newLayer)
		{
			rootGameObject.layer = newLayer;
			foreach (Transform childTransform in rootGameObject.transform)
			{
				SetEggLayerMaskRecursively(childTransform.gameObject, newLayer);
			}
		}

		bool _alreadyLayed = false;
		Vector3 _layPosition = new();
		float CurrentLayChargeRatio => Mathf.Clamp01(_layChargeElapsedTime / _layChargeMaxTime);
		float _layChargeElapsedTime = 0.0f;
		float _layChargeMaxTime = 1.0f;
		[SerializeField] float _rayCastMaxDistance = 10.0f;
		int _eggLayerMaskNormal = 8;
		int _eggLayerMaskPreparing = 9;
		EggFactory _eggFactory;
	}
}
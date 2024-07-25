using System.Collections;

using UnityEngine;

namespace MC
{

	[DisallowMultipleComponent]
	public partial class EggAction : ActionRoutineBase
	{
		#region Unity Callbacks

		#endregion

		public void BeginAction()
		{
			TryStopCurrentRoutine();

			if (!_eggPrefab)
			{
				return;
			}

			if (_alreadyLayedEgg)
			{
				return;
			}

			_currentRoutine = StartCoroutine(LayEggChargeRoutine());
		}

		public void EndAction()
		{
			TryStopCurrentRoutine();

			if (!_eggPrefab)
			{
				return;
			}
			if (_alreadyLayedEgg)
			{
				return;
			}

			PerformLayEgg();
		}

		IEnumerator LayEggChargeRoutine()
		{
			_currentLayEggChargeSeconds = 0.0f;
			while (CurrentLayEggChargeRatio < 1.0f)
			{
				_currentLayEggChargeSeconds += Time.deltaTime;
				yield return null;
			}

			PerformLayEgg();

			_currentLayEggChargeSeconds = 0.0f;
			_currentRoutine = null;

		}

		void PerformLayEgg()
		{
			var isCollide = Physics.Raycast
			(
				origin: transform.position,
				direction: -1.0f * transform.up,
				hitInfo: out var hitInfo,
				maxDistance: _rayCastMaxDistance,
				layerMask: gameObject.layer // 액션을 수행하는 자의 layerMask 는 무시
			);


			var instance = Instantiate(_eggPrefab, transform.position, Quaternion.identity);

			_spawnPosition = transform.position;

			if (isCollide)
			{
				_spawnPosition.y += instance.GetComponent<Egg>().CombinedBound.extents.y;
			}

			instance.transform.position = _spawnPosition;

			GetComponent<Rigidbody>()?.AddForce(transform.up * 10.0f, ForceMode.Impulse);

			// _alreadyLayedEgg = true;
		}

		bool _alreadyLayedEgg = false;
		Vector3 _spawnPosition = new();

		float CurrentLayEggChargeRatio => Mathf.Clamp01(_currentLayEggChargeSeconds / _maxLayEggChargeSeconds);
		float _currentLayEggChargeSeconds = 0.0f;
		float _maxLayEggChargeSeconds = 1.0f;
		[SerializeField] float _rayCastMaxDistance = 10.0f;

		[SerializeField] GameObject _eggPrefab;
	}
}
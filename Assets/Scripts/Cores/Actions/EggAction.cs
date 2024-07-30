using System.Collections;

using UnityEngine;

namespace MC
{

	[DisallowMultipleComponent]
	[RequireComponent(typeof(EggFactory))]
	[RequireComponent(typeof(Rigidbody))]
	public partial class EggAction : ActionRoutineBase
	{
		#region Unity Callbacks

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();

#if UNITY_EDITOR
			if (!_rigidbody)
			{
				Debug.LogWarning("Rigidbody 컴포넌트를 찾을 수 없습니다.");
			}
#endif

			_eggFactory = GetComponent<EggFactory>();

#if UNITY_EDITOR
			if (!_eggFactory)
			{
				Debug.LogWarning("EggFactory 컴포넌트를 찾을 수 없습니다.");
			}
#endif
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
			PerformLayActionRecoil();

			_layChargeElapsedTime = 0.0f;
			_currentRoutine = null;
		}

		void PerformLayAction()
		{
			_eggFactory.GetEggByCharacter
			(
				characterObject: gameObject,
				raycastDistance: _rayCastMaxDistance
			);
		}

		void PerformLayActionRecoil()
		{
			_rigidbody.AddForce(transform.up * 10.0f, ForceMode.Impulse);
		}

		Rigidbody _rigidbody;
		EggFactory _eggFactory;

		bool _alreadyLayed = false;
		Vector3 _layPosition = new();
		float CurrentLayChargeRatio => Mathf.Clamp01(_layChargeElapsedTime / _layChargeMaxTime);
		float _layChargeElapsedTime = 0.0f;
		float _layChargeMaxTime = 1.0f;
		[SerializeField] float _rayCastMaxDistance = 10.0f;
	}
}
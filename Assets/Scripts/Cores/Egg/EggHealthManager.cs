using System.Collections;

using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public partial class EggHealthManager : MonoBehaviour
	{
		delegate void EggDamagedEventHandler(in float damage);
		event EggDamagedEventHandler Damaged;

		public delegate void EggShouldDestroyedEventHandler();
		public event EggShouldDestroyedEventHandler HealthIsBelowZero;

		public delegate void EggHealthChangedEventHandler(in float healthRatio);
		public event EggHealthChangedEventHandler HealthChanged;

		#region Unity Callbacks

		void Awake()
		{
			_lifeCycleHandler = GetComponent<EggLifeCycleHandler>();

#if UNITY_EDITOR
			if (!_lifeCycleHandler)
			{
				Debug.LogWarning("EggLifeCycleHandler 컴포넌트를 찾을 수 없습니다.");
			}
#endif

			_impactDetector = GetComponent<EggImpactDetector>();

#if UNITY_EDITOR
			if (!_impactDetector)
			{
				Debug.LogWarning("EggImpactDetector 컴포넌트를 찾을 수 없습니다.");
			}
#endif
		}

		void OnEnable()
		{
			_lifeCycleHandler.Created += InitializeHealth;
			_lifeCycleHandler.Created += BlockDamageForCreation;
			_impactDetector.ImpactCrossedThreshold += TryInflictDamage;

			HealthIsBelowZero += OnHealthIsBelowZero;
			Damaged += InflictDamage;
		}

		void OnDisable()
		{
			_lifeCycleHandler.Created -= InitializeHealth;
			_lifeCycleHandler.Created -= BlockDamageForCreation;
			_impactDetector.ImpactCrossedThreshold -= TryInflictDamage;

			HealthIsBelowZero -= OnHealthIsBelowZero;
			Damaged -= InflictDamage;
		}

		#endregion // Unity Callbacks

		void InitializeHealth()
		{
			_currentHealth = _maxHealth;
			HealthChanged?.Invoke(HealthRatio);
		}

		void BlockDamageForCreation()
		{
			_currentTimerRoutine = StartCoroutine(BlockDamageTimerRoutine(_damageTimerMaxTimeForCreated));
		}

		/// <summary>
		/// 달걀이 데미지를 받을 수 있는 상태이면, 충격량을 데미지로 변환하여 적용한다. <br/>
		/// 받을 수 없는 상태라면, 아무것도 하지 않는다.
		/// </summary>
		void TryInflictDamage(in float impactForceMagnitude)
		{
			if (!_canBeDamaged)
			{
				return;
			}

			Damaged?.Invoke(ConvertImpactToDamage(impactForceMagnitude));
		}

		void InflictDamage(in float damage)
		{
			_currentHealth -= damage;

			HealthChanged?.Invoke(HealthRatio);

			if (_currentHealth <= 0.0f)
			{
				HealthIsBelowZero?.Invoke();
				return;
			}

			_currentTimerRoutine = StartCoroutine(BlockDamageTimerRoutine(_damageTimerMaxTime));
		}

		void OnHealthIsBelowZero()
		{
			_currentHealth = 0.0f;

			StopCoroutine(_currentTimerRoutine);
			_currentTimerRoutine = null;
		}

		IEnumerator BlockDamageTimerRoutine(float duration)
		{
			// Block Damage Start
			_damageTimerElapsedTime = 0.0f;
			_canBeDamaged = false;

			while (Mathf.Clamp01(_damageTimerElapsedTime / duration) < 1.0f)
			{
				_damageTimerElapsedTime += Time.deltaTime;
				yield return null;
			}

			// Block Damage End
			_damageTimerElapsedTime = 0.0f;
			_canBeDamaged = true;
		}

		/// <summary>
		/// 충격량을 데미지로 변환하는 공식
		/// </summary>
		float ConvertImpactToDamage(in float impactForceMagnitude)
		{
			return impactForceMagnitude * 2.0f;
		}

		EggLifeCycleHandler _lifeCycleHandler;
		EggImpactDetector _impactDetector;
		float HealthRatio => Mathf.Clamp01(_currentHealth / _maxHealth);
		float _currentHealth;
		[SerializeField] float _maxHealth = 100.0f;
		bool _canBeDamaged = true;
		float DamageBlockTimerProgress => Mathf.Clamp01(_damageTimerElapsedTime / _damageTimerMaxTime);
		float _damageTimerElapsedTime = 0.0f;
		[SerializeField] float _damageTimerMaxTime = 3.0f;
		[SerializeField] float _damageTimerMaxTimeForCreated = 1.0f;
		Coroutine _currentTimerRoutine;
	}
}
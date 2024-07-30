using System.Collections;

using UnityEngine;

namespace MC
{
	/// <summary>
	/// 알의 데미지 자극원들로부터 전달된 데미지를 알의 체력에 적용할 지 말 지 판단
	/// </summary>
	[DisallowMultipleComponent]
	public partial class EggHealthManager : MonoBehaviour
	{
		// (TryInflictDamage) -> InflictDamaged -> HealthChanged -> HealthIsBelowZero
		public delegate void DamagedEventHandler(in float damage);
		public event DamagedEventHandler Damaged;
		public delegate void HealthChangedEventHandler(in float healthRatio);
		public event HealthChangedEventHandler HealthChanged;
		public delegate void ShouldDestroyedEventHandler();
		public event ShouldDestroyedEventHandler ShouldEndLifecycle;

		#region UnityCallbacks

		void Awake()
		{
			// Cache components

			_lifeCycleHandler = GetComponent<EggLifecycleHandler>();

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
			Debug.Log("HealthManager Awake");

			// Bind events

			_lifeCycleHandler.LifecycleStarted += MaxUpHealth;
			_lifeCycleHandler.LifecycleStarted += BlockDamageForCreation;

			_impactDetector.ShouldInflictDamage += TryInflictDamage;

			ShouldEndLifecycle += OnHealthIsBelowZero;
			Damaged += InflictDamage;
		}

		void OnDestroy()
		{
			// Unbind events

			_lifeCycleHandler.LifecycleStarted -= MaxUpHealth;
			_lifeCycleHandler.LifecycleStarted -= BlockDamageForCreation;

			_impactDetector.ShouldInflictDamage -= TryInflictDamage;

			ShouldEndLifecycle -= OnHealthIsBelowZero;
			Damaged -= InflictDamage;
		}

		#endregion // UnityCallbacks

		void MaxUpHealth()
		{

#if UNITY_EDITOR
			Debug.Log($"{gameObject}'s health set to max health.");
#endif

			_currentHealth = _maxHealth;
			HealthChanged?.Invoke(HealthRatio);
		}

		void BlockDamageForCreation()
		{
			if (_currentTimerRoutine != null)
			{
				StopCoroutine(_currentTimerRoutine);
			}

			_currentTimerRoutine = StartCoroutine(BlockDamageTimerRoutine(_damageTimerMaxTimeForCreated));
		}

		void TryInflictDamage(in float damage)
		{
			if (!_canBeDamaged)
			{
				return;
			}

			Damaged?.Invoke(damage);
		}

		void InflictDamage(in float damage)
		{
			_currentHealth -= damage;

			HealthChanged?.Invoke(HealthRatio);

			if (_currentTimerRoutine != null)
			{
				StopCoroutine(_currentTimerRoutine);
			}

			if (_currentHealth <= 0.0f)
			{
				ShouldEndLifecycle?.Invoke();
				return;
			}

			_currentTimerRoutine = StartCoroutine(BlockDamageTimerRoutine(_damageTimerMaxTime));
		}

		IEnumerator BlockDamageTimerRoutine(float duration)
		{
			Debug.Log($"Damage Timer Started for duration {duration}");
			// 데미지 차단 시작 (타이머 시작)
			_damageTimerElapsedTime = 0.0f;
			_canBeDamaged = false;

			while (Mathf.Clamp01(_damageTimerElapsedTime / duration) < 1.0f)
			{
				_damageTimerElapsedTime += Time.deltaTime;
				yield return null;
			}

			// 데미지 차단 종료 (타이머 종료)
			_damageTimerElapsedTime = 0.0f;
			_canBeDamaged = true;
		}

		void OnHealthIsBelowZero()
		{
			_currentHealth = 0.0f;

			if (_currentTimerRoutine != null)
			{
				StopCoroutine(_currentTimerRoutine);
			}
			_currentTimerRoutine = null;
		}

		EggLifecycleHandler _lifeCycleHandler;
		EggImpactDetector _impactDetector;

		bool _canBeDamaged = true;
		float _damageTimerElapsedTime = 0.0f;
		[SerializeField] float _damageTimerMaxTime = 3.0f;
		[SerializeField] float _damageTimerMaxTimeForCreated = 1.0f;

		float HealthRatio => Mathf.Clamp01(_currentHealth / _maxHealth);
		float _currentHealth;
		[SerializeField] float _maxHealth = 100.0f;

		Coroutine _currentTimerRoutine;
	}
}

// NOTE ActionRoutineBase 와 아무런 관련이 없으나, 비슷한 구조임
// TODO 코루틴을 변수에 할당하는 경우 StopCoroutine 시 null이 되는지, 조사할 필요가 있다.
// 코루틴 관리가 필요해해해해
using System.Collections;

using UnityEngine;

namespace MC
{

/// <summary>
/// 데미지 요인들로부터 전달된 데미지 적용 요청을 적절히 처리하여 적용. <br/>
/// 생애주기 결정요인 중 하나이다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(EggLifecycleHandler))]
public partial class EggHealthManager : MonoBehaviour
{
	public delegate void DamagedEventHandler(float damage);
	public delegate void HealthChangedHandler(float resultHealthRatio);

	/// <summary>
	/// 데미지를 받을 때 마다 그 값을 전달받고 싶다면, 이것을 구독
	/// </summary>
	public event DamagedEventHandler Damaged;

	/// <summary>
	/// 체력이 변화할 때마다 그 값을 전달받고 싶다면, 이것을 구독
	/// </summary>
	public event HealthChangedHandler HealthChanged;


#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_eggLifecycleHandler = GetComponent<EggLifecycleHandler>();

		// Bind events

		_eggLifecycleHandler.LifecycleStarted += OnLifecycleStarted;
	}

	void OnDestroy()
	{
		// Unbind events

		_eggLifecycleHandler.LifecycleStarted -= OnLifecycleStarted;
	}

	#endregion // UnityCallbacks

	void OnLifecycleStarted()
	{
		_currentHealth = _maxHealth;
		HealthChanged?.Invoke(HealthRatio);

		// Damage blocking timer for creation phase
		StopAllCoroutines();
		StartCoroutine(BlockDamageTimerRoutine(_damageTimerMaxTimeForCreated));
	}

	/// <summary>
	/// 모든 데미지 요인들은 이 함수를 호출하여 데미지 적용 요청을 보낸다. <br/>
	/// 데미지를 적용할 수 있다면, 데미지를 적용하고 그렇지 않으면 무시한다.
	/// </summary>
	public void TryInflictDamage(float damage)
	{
		if (!_canBeDamaged)
		{
			return;
		}

		Damaged?.Invoke(damage);

		InflictDamage(damage);
	}

	/// <summary>
	/// 데미지를 받을 수 있는지의 여부와 관계없이, 데미지를 바로 적용한다.
	/// </summary>
	public void InflictDamage(float damage)
	{
		StopAllCoroutines();

		_currentHealth -= damage;

		HealthChanged?.Invoke(HealthRatio);

		if (_currentHealth <= 0.0f)
		{
			// Health is below zero, this egg "should be" return to pool
			_currentHealth = 0.0f;
			_eggLifecycleHandler.EndLifecycle(spawnBrokenEgg: true, grabber: null);

			return;
		}

		StartCoroutine(BlockDamageTimerRoutine(_damageTimerMaxTime));
	}

	IEnumerator BlockDamageTimerRoutine(float duration)
	{
		_currentDamageTimerMaxTime = duration;

		// Begin blocking damage
		_damageTimerElapsedTime = 0.0f;
		_canBeDamaged = false;

		while (Mathf.Clamp01(_damageTimerElapsedTime / _currentDamageTimerMaxTime) < 1.0f)
		{
			_damageTimerElapsedTime += Time.deltaTime;
			yield return null;
		}

		// End blocking damage
		_damageTimerElapsedTime = 0.0f;
		_canBeDamaged = true;

		_currentDamageTimerMaxTime = 0.0f;
	}

	float HealthRatio => Mathf.Clamp01(_currentHealth / _maxHealth);
	public float MaxHealth => _maxHealth;

	EggLifecycleHandler _eggLifecycleHandler;

	[HideInInspector][SerializeField] bool _canBeDamaged = true;
	[HideInInspector][SerializeField] float _damageTimerElapsedTime = 0.0f;
	[SerializeField] float _damageTimerMaxTime = 3.0f;
	[SerializeField] float _damageTimerMaxTimeForCreated = 1.0f;
	[HideInInspector][SerializeField] float _currentDamageTimerMaxTime;
	[HideInInspector][SerializeField] float _currentHealth;
	[SerializeField] float _maxHealth = 100.0f;
}

}
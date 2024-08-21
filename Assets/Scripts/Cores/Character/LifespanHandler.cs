using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{

/// <summary>
/// 캐릭터에게 최대 수명을 부여하고, 각 수명의 단계에 맞게 이벤트를 트리거시키는 컴포넌트.
/// </summary>
[DisallowMultipleComponent]
public class LifespanHandler : MonoBehaviour
{
	public enum ELifespanState
	{
		Running,
		Paused,
		Ended
	}

	public delegate void LifespanEventHandler();
	public delegate void LifespanChangedHandler(in float totalRatio, in float currentRatio);

	public event LifespanEventHandler Started;

	/// <summary>
	/// 외부에서 캐릭터의 수명과 노화의 변화율을 알고 싶으면 이것을 구독한다.
	/// </summary>
	public event LifespanChangedHandler Changed;

	/// <summary>
	/// 외부에서 캐릭터가 변이 한계에 도달한 시점을 알고 싶으면 이것을 구독한다.
	/// </summary>
	public event LifespanEventHandler ReachedMutationThreshold;

	/// <summary>
	/// 외부에서 캐릭터의 수명이 끝난 시점을 알고 싶으면 이것을 구독한다.
	/// </summary>
	public event LifespanEventHandler Ended;

#region UnityCallbacks

	void Awake()
	{
		SceneManager.activeSceneChanged += OnPersistentGameplaySceneLoadedAndActivated;
	}

	void OnDestroy()
	{
		// NOTE 구독 해제를 해야하는지 솔직히 확실하지 않지만 일단 하는 것으로
		SceneManager.activeSceneChanged -= OnPersistentGameplaySceneLoadedAndActivated;
	}

#endregion // UnityCallbacks

	void OnPersistentGameplaySceneLoadedAndActivated(Scene from, Scene to)
	{
		RestartLifespan();
	}

	// 영혼 상태가 아니라면 항상 돌아가고 있어야 함
	IEnumerator LifespanTimerRoutine(float startTime, float currentTimeOffset)
	{
		_currentLifespan = currentTimeOffset;
		_startTime = startTime;

		if (!_startedCalled)
		{
			Started?.Invoke();
			_startedCalled = true;
		}

		while (_currentLifespan < _maxLifespan)
		{
			_currentLifespan = Time.time - _startTime;

			if (_currentLifespan > _mutationThreshold && !_mutatedCalled)
			{
				ReachedMutationThreshold?.Invoke();
				_mutatedCalled = true;
			}

			Changed?.Invoke(LifespanRatio, AgingRatio);

			yield return new WaitForSecondsRealtime(_resolution);
		}

		_currentLifespan = _maxLifespan;

		if(!_endedCalled)
		{
			Ended?.Invoke();
			_endedCalled = true;
		}
	}

	/// <summary>
	/// 이벤트 호출을 초기화하고, 생애주기를 처음부터 다시 시작한다.
	/// </summary>
	public void RestartLifespan()
	{
		StopAllCoroutines();

		_startedCalled = false;
		_mutatedCalled = false;
		_endedCalled = false;

		_currentState = ELifespanState.Running;

		StartCoroutine(LifespanTimerRoutine(startTime: Time.time, currentTimeOffset: 0.0f));
	}

	/// <summary>
	/// 생애주기가 진행 중이라면, 정지한다.
	/// </summary>
	void PauseLifespan()
	{
		if (_currentState == ELifespanState.Running)
		{
			StopAllCoroutines();
			_currentState = ELifespanState.Paused;
		}
	}

	/// <summary>
	/// 생애주기가 정지 중이라면, 그 시점부터 다시 시작한다.
	/// </summary>
	void ResumeLifespan()
	{
		if (_currentState == ELifespanState.Paused)
		{
			StartCoroutine(LifespanTimerRoutine(startTime: Time.time - _currentLifespan, currentTimeOffset: _currentLifespan));
			_currentState = ELifespanState.Running;
		}
	}

	/// <summary>
	/// 생애주기를 종료한다.
	/// </summary>
	void EndLifespan()
	{
		if (_currentState != ELifespanState.Ended)
		{
			StopAllCoroutines();
			_currentLifespan = _maxLifespan;
			if (!_endedCalled)
			{
				Ended?.Invoke();
				_endedCalled = true;
			}
			_currentState = ELifespanState.Ended;
		}
	}

	public float LifespanRatio => _currentLifespan / _maxLifespan;

	/// <summary>
	/// 변이를 하지 않았다면 다음 변이까지, 변이를 이미 하였다면 수명 종료까지 얼마나 진행되었는지를 반환
	/// </summary>
	public float AgingRatio => !_mutatedCalled
		? Mathf.Clamp01(_currentLifespan / _mutationThreshold)
		: Mathf.Clamp01((_currentLifespan - _mutationThreshold) / (_maxLifespan - _mutationThreshold));


	bool _startedCalled = false;
	bool _mutatedCalled = false;
	bool _endedCalled = false;
	[SerializeField][HideInInspector] ELifespanState _currentState;
	[SerializeField] float _resolution = 0.1f;
	float _startTime;
	[SerializeField][HideInInspector] float _currentLifespan = 0.0f;
	[SerializeField] float _maxLifespan = 30.0f;
	[SerializeField] float _mutationThreshold = 15.0f;
}

}
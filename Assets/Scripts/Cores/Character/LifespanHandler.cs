using System.Collections;

using UnityEngine;

namespace MC
{

/// <summary>
/// 캐릭터에게 최대 수명을 부여하고, 각 수명의 단계에 맞게 이벤트를 트리거시키는 컴포넌트.
/// </summary>
[DisallowMultipleComponent]
public class LifespanHandler : MonoBehaviour
{
	public delegate void LifespanEventHandler();
	public delegate void LifespanChangedHandler(in float totalRatio, in float currentRatio);

	public event LifespanEventHandler Started;
	public event LifespanChangedHandler Changed;
	public event LifespanEventHandler ReachedMutationThreshold;
	public event LifespanEventHandler Ended;

#region UnityCallbacks

	void Start()
	{
		StartCoroutine(LifespanTimerRoutine());
	}

#endregion // UnityCallbacks

	IEnumerator LifespanTimerRoutine()
	{
		_currentLifespan = 0.0f;
		_isMutated = false;

		Started?.Invoke();

		var startTime = Time.time;

		while (_currentLifespan < _maxLifespan)
		{
			_currentLifespan = Time.time - startTime;

			if (_currentLifespan > _mutationThreshold && !_isMutated)
			{
				_isMutated = true;
				ReachedMutationThreshold?.Invoke();
			}

			Changed?.Invoke(LifespanRatio, AgingRatio);

			yield return new WaitForSecondsRealtime(_ageEvaluationResolution);
		}

		_currentLifespan = _maxLifespan;
		Ended?.Invoke();

		// NOTE 임시로 수명을 재반복 하도록 설정하였음
		// TODO 원래라면 SpanEnded에서 (1) 영혼 캐릭터가 되고, (2) 최적의 부활 지점으로 이동 해야함
		// 스스로를 재귀 호출 하기 때문에, _maxLifeSpan이 낮은 값인 경우 스택이 터질 수 있어서 한 프레임이 완료될 때가지 대기.
		yield return null;
		StartCoroutine(LifespanTimerRoutine());
	}

	public float LifespanRatio => _currentLifespan / _maxLifespan;

	/// <summary>
	/// 변이를 하지 않았다면 다음 변이까지, 변이를 이미 하였다면 수명 종료까지 얼마나 진행되었는지를 반환
	/// </summary>
	public float AgingRatio => !_isMutated
		? Mathf.Clamp01(_currentLifespan / _mutationThreshold)
		: Mathf.Clamp01((_currentLifespan - _mutationThreshold) / (_maxLifespan - _mutationThreshold));
	bool _isMutated = false;
	[SerializeField] float _ageEvaluationResolution = 0.1f;
	[HideInInspector] [SerializeField] float _currentLifespan = 0.0f;
	[SerializeField] float _maxLifespan = 30.0f;
	[SerializeField] float _mutationThreshold = 15.0f;
}

}
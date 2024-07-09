using System;
using System.Collections;

using UnityEngine;

namespace MC
{
	/// <summary>
	/// 캐릭터에게 최대 수명을 부여하고, 각 수명의 단계에 맞게 이벤트를 트리거시키는 컴포넌트.
	/// </summary>
	[DisallowMultipleComponent]
	public partial class LifespanHandler : MonoBehaviour
	{
		#region Public Static Events

		/// <summary> 수명을 먹기 시작했을 때</summary>
		public static event Action LifespanStarted;

		/// <summary> 수명이 변화할 때 </summary>
		/// <remarks> 전체 수명 비율과, 현재 노화 비율을 전달 </remarks>
		public static event Action<float, float> LifespanChanged;

		/// <summary> 수명이 변이 한계에 도달했을 때 </summary>
		public static event Action LifespanReachedMutationThreshold;

		/// <summary> 수명이 최대치에 도달했을 때 </summary>
		public static event Action LifespanEnded; // 수명이 종료되어 생애주기가 끝났을 때

		#endregion // Public Static Events

		#region Unity Messages

		// void Awake() {}

		void OnEnable()
		{
#if UNITY_EDITOR
			if (_logOnLifespanEvent)
			{
				LifespanStarted += () => { Debug.Log("<color='yellow'>Lifespan Started!</color>"); };
				LifespanEnded += () => { Debug.Log("<color='red'>Lifespan Ended!</color>"); };
				LifespanReachedMutationThreshold += () => { Debug.Log("<color='pink'>Lifespan reached mutation threshold!</color>"); };
			}
#endif
		}

		void Start()
		{
			StartCoroutine(LifespanRoutine());
		}

		void OnDisable()
		{

#if UNITY_EDITOR
			if (_logOnLifespanEvent)
			{
				LifespanStarted = null;
				LifespanEnded = null;
				LifespanReachedMutationThreshold = null;
			}
#endif
		}

		#endregion // Unity Messages

		/// <summary>
		/// 수명 타이머, 각 시점에 맞게 이벤트를 호출해준다.
		/// </summary>
		IEnumerator LifespanRoutine()
		{
			_currentLifespan = 0.0f;
			_isAlreadyMutated = false;

			LifespanStarted?.Invoke();

			var startTime = Time.time;

			while (_currentLifespan < _maxLifespan)
			{
				_currentLifespan = Time.time - startTime;

				if (IsOverMutationThreshold && !_isAlreadyMutated)
				{
					_isAlreadyMutated = true;
					LifespanReachedMutationThreshold?.Invoke();
				}

				LifespanChanged?.Invoke(LifespanRatio, AgingRatio);

				yield return new WaitForSecondsRealtime(_ageEvaluationResolution);
			}

			_currentLifespan = _maxLifespan;
			LifespanEnded?.Invoke();

			// NOTE 임시로 수명을 재반복 하도록 설정하였음
			// TODO 원래라면 SpanEnded에서 (1) 영혼 캐릭터가 되고, (2) 최적의 부활 지점으로 이동 해야함
			// 스스로를 재귀 호출 하기 때문에, _maxLifeSpan이 낮은 값인 경우 스택이 터질 수 있어서 한 프레임이 완료될 때가지 대기.
			yield return null;
			StartCoroutine(LifespanRoutine());
		}

		bool IsOverMutationThreshold => _currentLifespan > _mutationThreshold;
		bool _isAlreadyMutated = false;
		float _currentLifespan = 0.0f;

		public float LifespanRatio => _currentLifespan / _maxLifespan;

		/// <summary> 변이를 하지 않았다면 다음 변이까지, 변이를 이미 하였다면 수명 종료까지 얼마나 진행되었는지를 반환함. </summary>
		/// <remarks> 병아리의 생이 얼마나 진행되었는가, 닭으로서의 생이 얼마나 진행되었는가 라고 생각하면 쉬움. </remarks>
		public float AgingRatio =>
			!_isAlreadyMutated
				? _currentLifespan / _mutationThreshold
				: (_currentLifespan - _mutationThreshold) / (_maxLifespan - _mutationThreshold);

		[Header("수명은 모두 초 단위로 계산합니다.")]

		[SerializeField] float _ageEvaluationResolution = 0.1f;
		[SerializeField] float _maxLifespan = 5.0f;
		[SerializeField] float _mutationThreshold = 3.0f;

#if UNITY_EDITOR
		[SerializeField] bool _logOnLifespanEvent = false;
#endif
	}

} // namespace
using UnityEngine;

namespace MC
{
	/// <summary>
	/// Egg의 생성(활성화)과 소멸(비활성화)에 관련된 이벤트를 관리한다.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EggFactory))]
	public class EggLifecycleHandler : MonoBehaviour
	{
		public delegate void LifecycleEventHandler();

		/// <summary>
		/// Egg가 풀에서 꺼내질 때
		/// </summary>
		public LifecycleEventHandler LifecycleStarted;

		/// <summary>
		/// Egg가 풀에 반납될 때
		/// </summary>
		public LifecycleEventHandler LifecycleEnded;

		#region UnityCallbacks

		void Awake()
		{
			// Cache components

			_factory = GetComponent<EggFactory>();

#if UNITY_EDITOR
			if (!_factory)
			{
				Debug.LogWarning("EggFactory 컴포넌트를 찾을 수 없습니다.");
			}
#endif

			_healthManager = GetComponent<EggHealthManager>();

#if UNITY_EDITOR
			if (!_healthManager)
			{
				Debug.LogWarning("EggHealthManager 컴포넌트를 찾을 수 없습니다.");
			}
#endif

			// Bind events

			_healthManager.ShouldEndLifecycle += LifecycleShouldEnded;
		}

		/// <remarks>
		/// Egg가 풀에서 꺼내질 때, 명시적으로 호출된다.
		/// </remarks>
		public void Initialize()
		{
			// 여기에 바인드된 이벤트들의 실행 순서를 통제할 수 없음.
			LifecycleStarted?.Invoke();
		}

		void OnDisable()
		{
			LifecycleEnded?.Invoke();
		}

		void OnDestroy()
		{
			// Unbind events

			_healthManager.ShouldEndLifecycle -= LifecycleShouldEnded;
		}

		#endregion // UnityCallbacks
		void LifecycleShouldEnded()
		{
			LifecycleEnded?.Invoke();

			_factory.ReturnEgg(this);
		}

		EggHealthManager _healthManager;
		EggFactory _factory;
	}
}
using UnityEngine;

namespace MC
{
	/// <summary>
	/// 달걀 객체의 생성(활성화)과 소멸(비활성화)에 관련된 이벤트를 관리한다.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EggFactory))]
	public class EggLifecycleHandler : MonoBehaviour
	{
		public delegate void LifecycleEventHandler();

		/// <summary>
		/// 달걀이 풀에서 꺼내질 때
		/// </summary>
		public LifecycleEventHandler LifecycleStarted;

		/// <summary>
		/// 달걀이 풀에 반납될 때
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
			Debug.Log("LifeCycleHandler Awake");

			// Bind events

			LifecycleStarted += OnLifecycleStarted;
			LifecycleEnded += OnLifecycleEnded;

			_healthManager.ShouldEndLifecycle += LifecycleShouldEnded;
		}

		void OnEnable()
		{
		}

		public void Initialize()
		{
			LifecycleStarted?.Invoke();
		}

		void OnDisable()
		{
			LifecycleEnded?.Invoke();
		}

		void OnDestroy()
		{
			// Unbind events

			LifecycleStarted -= OnLifecycleStarted;
			LifecycleEnded -= OnLifecycleEnded;

			_healthManager.ShouldEndLifecycle -= LifecycleShouldEnded;
		}

		#endregion // UnityCallbacks

		void OnLifecycleStarted()
		{
			Debug.Log("OnLifecycleStarted Invoked");
		}

		void OnLifecycleEnded()
		{
			Debug.Log("OnLifecycleEnded Invoked");
		}

		void LifecycleShouldEnded()
		{
			LifecycleEnded?.Invoke();

			_factory.ReturnEgg(this);
		}

		EggHealthManager _healthManager;
		EggFactory _factory;
	}
}
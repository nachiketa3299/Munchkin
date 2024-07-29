using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EggFactory))]
	public partial class EggLifeCycleHandler : MonoBehaviour
	{
		public delegate void EggCreatedEventHandler();
		public EggCreatedEventHandler Created;
		delegate void EggDestroyedEventHandler();
		EggDestroyedEventHandler Destroyed;

		#region Unity Callbacks

		void Awake()
		{
			_eggFactory = GetComponent<EggFactory>();
#if UNITY_EDITOR
			if (!_eggFactory)
			{
				Debug.LogWarning("EggFactory 컴포넌트를 찾을 수 없습니다.");
			}
#endif

			_eggHealthManager = GetComponent<EggHealthManager>();

#if UNITY_EDITOR
			if (!_eggHealthManager)
			{
				Debug.LogWarning("EggHealthManager 컴포넌트를 찾을 수 없습니다.");
			}
#endif
		}

		void OnEnable()
		{
			Created += OnEggCreated;
			Destroyed += OnEggDestroyed;
			_eggHealthManager.HealthIsBelowZero += OnEggShouldDestroyed;
		}

		void Start()
		{
			Created?.Invoke();
		}

		void OnDisable()
		{
			Created -= OnEggCreated;
			Destroyed -= OnEggDestroyed;
			_eggHealthManager.HealthIsBelowZero -= OnEggShouldDestroyed;
		}

		#endregion // Unity Callbacks

		void OnEggCreated()
		{
			Debug.Log("Yeah ~ I'm created ~!");
		}

		void OnEggDestroyed()
		{
			Debug.Log("Yeah ~ I'm destroyed ~!");
		}

		void OnEggShouldDestroyed()
		{
			Destroyed?.Invoke();

			// Destroy(gameObject);

			_eggFactory.Despawn(this);
		}
		EggHealthManager _eggHealthManager;
		EggFactory _eggFactory;
	}
}
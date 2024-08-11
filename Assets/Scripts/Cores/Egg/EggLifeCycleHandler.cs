using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace MC
{

/// <summary>
/// Egg의 생성(활성화)과 소멸(비활성화)에 관련된 이벤트를 관리한다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EggFactory))]
[RequireComponent(typeof(BrokenEggFactory))]
[RequireComponent(typeof(EggHealthManager))]
public class EggLifecycleHandler : MonoBehaviour
{
	public delegate void LifecycleStartedHandler();

	/// <remarks>
	/// 생애주기를 종료할 때, 이 알의 운동 정보가 그대로 깨진 알의 운동 정보에 복사되어야 하므로, 함께 전달되어야 한다.
	/// </remarks>
	public delegate void LifecycleEndedHandler(in FEggPhysicalState lastPhysicalState);
	public event LifecycleStartedHandler LifecycleStarted;
	public event LifecycleEndedHandler LifecycleEnded;
	public LifecycleEndedHandler LifecycleEndedByTriggerEvent;

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();

		_factory = GetComponent<EggFactory>();
		_brokenEggFactory = GetComponent<BrokenEggFactory>();
		_healthManager = GetComponent<EggHealthManager>();

		// Bind events

		LifecycleEnded += OnLifecycleEnded;
		LifecycleEndedByTriggerEvent += OnLifecycleEndedByTriggerEvent;
		_healthManager.ShouldEndLifecycle += LifecycleShouldEnded;
	}

	void OnDestroy()
	{
		// Unbind events

		LifecycleEnded -= OnLifecycleEnded;
		LifecycleEndedByTriggerEvent -= OnLifecycleEndedByTriggerEvent;
		_healthManager.ShouldEndLifecycle -= LifecycleShouldEnded;
	}

	void Update()
	{
		if (!_alreadyEndedLifecycleByTrigger)
		{
			return;
		}

		LifecycleEnded?.Invoke(_cachedEggPhysicalState);
	}

#endregion // UnityCallbacks

	/// <remarks>
	/// Egg가 풀에서 꺼내질 때, <see cref="EggFactory"/>에 의해 명시적으로 호출된다.
	/// </remarks>
	public void InitializeLifecycle(in EEggOwner owner)
	{
		_owner = owner;
		_alreadyEndedLifecycleByTrigger = false;

#if UNITY_EDITOR
		gameObject.name = MakeInstanceName(owner);
#endif

		LifecycleStarted?.Invoke();
	}

	/// <remark>
	/// Egg가 복합 콜라이더인 경우, 한 물리 프레임 내에 OnTrigger 이벤트가 여러 번 호출될 수 있음. <br/>
	/// 따라서 물리 트리거에 의해 파괴가 결정된 경우, 다음 비물리 프레임 업데이트에서 파괴를 처리하는 것으로
	/// </remark>
	public void OnLifecycleEndedByTriggerEvent(in FEggPhysicalState lastEggPhysicalState)
	{
		if (_alreadyEndedLifecycleByTrigger)
		{
			return;
		}

		_alreadyEndedLifecycleByTrigger = true;
		_cachedEggPhysicalState = lastEggPhysicalState;
	}

	public void OnLifecycleEnded(in FEggPhysicalState lastEggPhysicalState)
	{
		_alreadyEndedLifecycleByTrigger = true;

		_brokenEggFactory.SpawnInitializedBrokenEggFromPool(lastEggPhysicalState);
		_factory.ReturnEggToPool(this);
	}

	public void LifecycleShouldEnded()
	{
		LifecycleEnded?.Invoke(LastPhysicalState);
		_factory.ReturnEggToPool(this);
	}

	public EEggOwner Owner => _owner;
	public FEggPhysicalState LastPhysicalState => new (this);
	public Rigidbody Rigidbody => _rigidbody;

	Rigidbody _rigidbody;
	EggFactory _factory;
	BrokenEggFactory _brokenEggFactory;
	EggHealthManager _healthManager;
	[SerializeField][HideInInspector] EEggOwner _owner;
	bool _alreadyEndedLifecycleByTrigger = false;
	FEggPhysicalState _cachedEggPhysicalState;

#if UNITY_EDITOR

	/// <summary>
	/// <paramref name="owner"/>가 매 생애주기 마다 바뀌므로, 팩토리나 풀이 아닌, 여기에서 주기가 시작될 때마다 이름을 재설정 해주어야 함.
	/// </summary>
	string MakeInstanceName(in EEggOwner owner) => $"{owner}Egg ({gameObject.GetInstanceID()})";

	[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
	static void DrawUniqueNameOfEgg(EggLifecycleHandler target, GizmoType gizmoType)
	{
		var style = new GUIStyle();
		{
			style.normal.textColor = Color.yellow;
			style.alignment = TextAnchor.MiddleCenter;
		}

		Handles.Label
		(
			position: target.transform.position,
			text: target.gameObject.name,
			style: style
		);
	}

#endif

}

}
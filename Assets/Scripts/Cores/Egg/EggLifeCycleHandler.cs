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
[RequireComponent(typeof(EggFactory))]
[RequireComponent(typeof(EggHealthManager))]
public class EggLifecycleHandler : MonoBehaviour
{
	public delegate void LifecycleEventHandler();
	public event LifecycleEventHandler LifecycleStarted;
	public event LifecycleEventHandler LifecycleEnded;

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_factory = GetComponent<EggFactory>();
		_healthManager = GetComponent<EggHealthManager>();

		// Bind events

		_healthManager.ShouldEndLifecycle += LifecycleShouldEnded;
	}

	/// <remarks>
	/// Egg가 풀에서 꺼내질 때, <see cref="EggFactory"/>에 의해 명시적으로 호출된다.
	/// </remarks>
	public void InitializeLifecycle(in EEggOwner owner)
	{
		_owner = owner;

#if UNITY_EDITOR
		gameObject.name = MakeInstanceName(owner);
#endif

		LifecycleStarted?.Invoke();
	}

	void OnDestroy()
	{
		// Unbind events

		_healthManager.ShouldEndLifecycle -= LifecycleShouldEnded;
	}

#endregion // UnityCallbacks

	public void LifecycleShouldEnded()
	{
		LifecycleEnded?.Invoke();

		_factory.ReturnEggToPool(this);
	}

	public EEggOwner Owner => _owner;

	EggFactory _factory;
	EggHealthManager _healthManager;
	[SerializeField][HideInInspector] EEggOwner _owner;

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
using UnityEngine;

namespace MC
{

/// <summary>
/// Egg의 생성(활성화)과 소멸(비활성화)에 관련된 이벤트를 관리한다. <br/>
/// Egg와 관련되어 가장 중요하고, 대표자로서 사용되는 클래스이다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BrokenEggFactory))]
public partial class EggLifecycleHandler : MonoBehaviour
{
	public delegate void LifecycleStartedHandler();
	public event LifecycleStartedHandler LifecycleStarted;

	public delegate void LifecycleEndedHandler(EggLifecycleHandler endingEgg);
	public event LifecycleEndedHandler LifecycleEnded;

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();
		_brokenEggFactory = GetComponent<BrokenEggFactory>();

#if UNITY_EDITOR
		if (!_eggPool)
		{
			Debug.LogWarning("EggLifecycleHandler에 RuntimePooledEggData가 설정되지 않았습니다.");
		}
#endif
	}

#endregion // UnityCallbacks

	public void Initialize(EEggOwner owner)
	{
		_owner = owner;

#if UNITY_EDITOR
		gameObject.name = MakeInstanceName(owner);
#endif

		_rigidbody.isKinematic = false;

		LifecycleStarted?.Invoke();
	}

	public void Deinitialize()
	{
		if (transform.parent != null)
		{
			transform.SetParent(null);
		}

		_rigidbody.isKinematic = true;
	}

	/// <summary>
	/// 모든 생애주기 종료 요인들은 이 함수를 호출하여 생애주기 종료 요청을 보낸다. <br/>
	/// </summary>
	public void EndLifecycle(bool spawnBrokenEgg = true, GrabThrowAction grabber = null)
	{
		LifecycleEnded?.Invoke(this);

		if (spawnBrokenEgg)
		{
			_brokenEggFactory.TakeFromPool(new EggLastState(egg: this, grabber: null));
		}

		_eggPool.Release(this);
	}

	public bool IsCharacterEgg => _owner == EEggOwner.Character;
	public bool IsNestEgg => _owner == EEggOwner.Nest;
	public EEggOwner Owner => _owner;
	public Vector3 Velocity => _rigidbody.velocity;
	public Vector3 AngularVelocity => _rigidbody.angularVelocity;

	Rigidbody _rigidbody;
	BrokenEggFactory _brokenEggFactory;

	[SerializeField][HideInInspector] EEggOwner _owner;
	[SerializeField] RuntimePooledEggData _eggPool;
}

}
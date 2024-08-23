using System;

using UnityEngine;

namespace MC
{

/// <summary>
/// Egg의 생성(활성화)과 소멸(비활성화)에 관련된 이벤트를 관리한다. <br/>
/// Egg와 관련되어 가장 중요하고, 대표자로서 사용되는 클래스이다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public partial class EggLifecycleHandler : MonoBehaviour
{
	public event Action LifecycleStarted;
	public event Action<EggLifecycleHandler> LifecycleEnded;

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();
	}

#endregion // UnityCallbacks

// Pool => Initialize => LifecycleStart => LifecycleEnd => Deinitialize => Pool

	/// <summary>
	/// <paramref name="owner"/>가 매 생애주기 마다 바뀌므로, 팩토리나 풀이 아닌, 여기에서 주기가 시작될 때마다 이름을 재설정 해주어야 함.
	/// </summary>
	string MakeInstanceName(in EEggOwner owner) => $"{owner}Egg ({gameObject.GetInstanceID()})";

	public void Initialize(EEggOwner owner)
	{
		gameObject.name = MakeInstanceName(owner);

		_owner = owner;

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
			BrokenEggPool.Instance.GetBrokenEggInstance(new EggLastState(egg: this, grabber: null));
		}

		EggPool.Instance.ReleaseEggInstance(this);
	}

	public bool IsCharacterEgg => _owner == EEggOwner.Character;
	public bool IsNestEgg => _owner == EEggOwner.Nest;
	public EEggOwner Owner => _owner;
	public Vector3 Velocity => _rigidbody.velocity;
	public Vector3 AngularVelocity => _rigidbody.angularVelocity;

	Rigidbody _rigidbody;

	[SerializeField][HideInInspector] EEggOwner _owner;
}

}
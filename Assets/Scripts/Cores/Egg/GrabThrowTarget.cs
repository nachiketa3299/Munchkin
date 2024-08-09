using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class GrabThrowTarget : MonoBehaviour
{
	public delegate void GrabThrowTargetDisabledEventHandler();

	/// <summary>
	/// 그랩 가능한 물체가 어떤 연유에서든 비활성화 되는 시점을 알고 싶을 때, 구독한다.
	/// </summary>
	public event GrabThrowTargetDisabledEventHandler GrabThrowTargetDisabled;

#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();
	}

	void OnDisable()
	{
		GrabThrowTargetDisabled?.Invoke();
	}

#endregion // UnityCallbacks

	public void BeginGrabState()
	{
		_rigidbody.isKinematic = true;
		_rigidbody.detectCollisions = false; // TODO Collision을 아예 꺼버리는 것이 아니라, 레이어를 바꾸는 것이 나을 것
	}

	public void EndGrabState()
	{
		_rigidbody.isKinematic = false;
		_rigidbody.detectCollisions = true;
	}

	public void AddForce(in Vector3 force)
	{
		_rigidbody.AddForce(force, ForceMode.VelocityChange);
	}

	public Rigidbody Rigidbody => _rigidbody;

	Rigidbody _rigidbody;
}

}
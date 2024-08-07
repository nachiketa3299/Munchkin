using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class GrabThrowTarget : MonoBehaviour
{
	public delegate void GrabThrowTargetDisabledEventHandler();
	public GrabThrowTargetDisabledEventHandler GrabThrowTargetDisabled;

	#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_rigidbody = GetComponent<Rigidbody>();

		// Bind events
	}

	void OnDisable()
	{
		GrabThrowTargetDisabled?.Invoke();
	}

	#endregion // UnityCallbacks

	public void BeginGrabState()
	{
		_rigidbody.isKinematic = true;
		_rigidbody.detectCollisions = false;

		_isGrabState = true;
	}

	public void EndGrabState()
	{
		_rigidbody.isKinematic = false;
		_rigidbody.detectCollisions = true;

		_isGrabState = false;
	}

	public void AddForce(in Vector3 force)
	{
		_rigidbody.AddForce(force, ForceMode.VelocityChange);
	}

	bool _isGrabState = false;
	public Rigidbody Rigidbody => _rigidbody;
	Rigidbody _rigidbody;
}

}
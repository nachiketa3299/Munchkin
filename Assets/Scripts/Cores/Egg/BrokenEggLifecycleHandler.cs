using System.Collections;
using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public partial class BrokenEggLifecycleHandler : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{
		// Cache Components

		_childRigidbodies = GetComponentsInChildren<Rigidbody>();
		_childTransforms = GetComponentsInChildren<Transform>();

	#if UNITY_EDITOR
		if (_childRigidbodies.Length == 0)
		{
			Debug.LogWarning("아무런 Rigidbody도 찾을 수 없습니다.");
		}
	#endif

	}

#endregion // UnityCallbacks

	// why should I use object pool while I can just Instantiate it.. ?

	string MakeInstanceName() => $"BrokenEgg ({gameObject.GetInstanceID()})";

	public void Initialize(EggLastState lastPhysicalState)
	{
		gameObject.name = MakeInstanceName();

		// Reset transform
		foreach (var tr in _childTransforms)
		{
			// NOTE 현재 Broken Egg와 그냥 Egg의 Rotation이 뒤틀려 있기 때문에, 최상위 로테이션은 보존하는것으로 함
			if (tr.parent == null)
			{
				continue;
			}

			tr.localPosition = Vector3.zero;
			tr.localEulerAngles = Vector3.zero;
		}

		transform.SetPositionAndRotation
		(
			position: lastPhysicalState.lastPosition,
			rotation: lastPhysicalState.lastRotation
		);

		foreach(var rigidbody in _childRigidbodies)
		{
			rigidbody.isKinematic = false;

			rigidbody.velocity = lastPhysicalState.lastVelocity;
			rigidbody.angularVelocity = lastPhysicalState.lastAngularVelocity;
		}

		StartCoroutine(LifecycleTimerRoutine());
	}

	public void Deinitialize()
	{
		foreach(var rigidbody in _childRigidbodies)
		{
			rigidbody.isKinematic = true;
		}
	}

	IEnumerator LifecycleTimerRoutine()
	{
		_currentLifespan = 0.0f;

		while (_currentLifespan < _maxLifespan)
		{
			_currentLifespan += Time.deltaTime;
			yield return null;
		}

		_currentLifespan = 0.0f;
		EndLifecycle();
	}

	void EndLifecycle()
	{
		BrokenEggPool.Instance.ReleaseBrokenEggInstance(this);
	}

	Rigidbody[] _childRigidbodies;
	Transform[] _childTransforms;

	[SerializeField] float _maxLifespan = 3.0f;
	[SerializeField][HideInInspector] float _currentLifespan = 0.0f;

}

}
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace MC
{

[DisallowMultipleComponent]
public class BrokenEggLifecycleHandler : MonoBehaviour
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

	// Check Data

#if UNITY_EDITOR
	if (!_runtimePooledBrokenEggData)
	{
		Debug.LogWarning("Runtime Pooled Broken Egg Data를 찾을 수 없습니다.");
	}
#endif

}

#endregion // UnityCallbacks

// This so sucks..
// why should I use object pool while I can just Instantiate it..

public void Initialize(EggLastState lastPhysicalState)
{
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

}

IEnumerator LifecycleTimerRoutine()
{
	_elapsedTime = 0.0f;

	while (_elapsedTime < _lifespan)
	{
		_elapsedTime += Time.deltaTime;
		yield return null;
	}

	_elapsedTime = 0.0f;
	ReturnBrokenEggToPool(this);
}

public void ReturnBrokenEggToPool(BrokenEggLifecycleHandler brokenEgg)
{
	foreach(var rigidbody in _childRigidbodies)
	{
		rigidbody.isKinematic = true;
	}

	_runtimePooledBrokenEggData.Pool.Release(brokenEgg);
}

Rigidbody[] _childRigidbodies;
Transform[] _childTransforms;
[SerializeField] RuntimePooledBrokenEggData _runtimePooledBrokenEggData;
[SerializeField] float _lifespan = 3.0f;
float _elapsedTime = 0.0f;

#if UNITY_EDITOR

[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
static void DrawBrokenEggGizmos(BrokenEggLifecycleHandler target, GizmoType gizmoType)
{
	if (target._childTransforms == null)
	{
		return;
	}

	var pivotPos = target.transform.position;
	var style = new GUIStyle();
	var gizmoColor = Color.gray;
	Gizmos.color = gizmoColor;

	style.alignment = TextAnchor.MiddleCenter;
	style.normal.textColor = gizmoColor;

	Handles.Label(pivotPos, $"{target.gameObject.name}", style);

	foreach(var childTransform in target._childTransforms)
	{
		var targetPos = childTransform.position;
		Gizmos.DrawLine(pivotPos, targetPos);
	}
}

#endif

}

}
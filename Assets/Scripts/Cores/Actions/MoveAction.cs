using System;
using System.Collections;

using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class MoveAction : ActionRoutineBase
{

#region UnityCallbacks

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

#endregion // UnityCallbacks

	/// <summary>
	/// <paramref name="directionValue"/> 방향으로 캐릭터의 가속도로 가속한다.
	/// </summary>
	public void BeginAction(float directionValue)
	{
		_directionValue = directionValue;
		TryStopCurrentRoutine();
		_currentRoutine = StartCoroutine(HorizontalAccelerationRoutine());
	}

	/// <summary>
	/// 정지할 때 까지 캐릭터의 감속도로 감속한다.
	/// </summary>
	public void EndAction()
	{
		TryStopCurrentRoutine();
		_currentRoutine = StartCoroutine(HorizontalDecelerationRoutine());
	}

	IEnumerator HorizontalAccelerationRoutine()
	{
		while (true)
		{
			if (Mathf.Abs(_rigidbody.velocity.x) < _maxHorizontalSpeed)
			{
				var force = _directionValue * _accelerationMagnitude * Vector3.right;
				_rigidbody.AddForce(force, ForceMode.Acceleration);
			}

			yield return new WaitForFixedUpdate();
		}
	}

	IEnumerator HorizontalDecelerationRoutine()
	{
		while (Mathf.Abs(_rigidbody.velocity.x) > 0.01f)
		{
			var force = -1.0f * _decelerationMagnitude * Mathf.Sign(_rigidbody.velocity.x) * Vector3.right;
			_rigidbody.AddForce(force, ForceMode.Acceleration);

			yield return new WaitForFixedUpdate();
		}

		_rigidbody.velocity.Set(0.0f, 0.0f, 0.0f);
	}

	Rigidbody _rigidbody;
	float _directionValue;

	// TODO 지면에 있는 경우 / 체공해 있는 경우 수평 가/감속도가 달라야 함.
	// TODO 또한, 캐릭터의 종류별로 아래 멤버의 값이 달라야 함. 캐릭터 오브젝트가 커지는 경우 각 멤버를 캐릭터별로 어떻게 저장하고 초기화할 지 고민할 것.
	[SerializeField] float _maxHorizontalSpeed = 10.0f;
	[SerializeField][Range(1.0f, 100.0f)] float _accelerationMagnitude = 10.0f;
	[SerializeField][Range(1.0f, 100.0f)] float _decelerationMagnitude = 10.0f;
}

}

using System.Collections;

using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class JumpAction : ActionRoutineBase
{

#region UnityCallbacks

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

#endregion // UnityCallbacks

	public void BeginAction()
	{
		TryStopCurrentRoutine();

		_currentRoutine = StartCoroutine(JumpChargeRoutine());
	}

	public void EndAction()
	{
		TryStopCurrentRoutine();
		PerformJump(CurrentJumpChargeRatio);
	}

	/// <summary>
	/// 점프 버튼이 떼어지기 전까지 점프를 누른 시간 동안 <see cref="_currentJumpChargeSeconds"/>를 충전한다.
	/// </summary>
	/// <remarks>
	/// 최대 충전을 넘어서는 경우 바로 점프한다.
	/// </remarks>
	IEnumerator JumpChargeRoutine()
	{
		_currentJumpChargeSeconds = 0.0f;
		while (CurrentJumpChargeRatio < 1.0f)
		{
			_currentJumpChargeSeconds += Time.deltaTime;
			yield return null;
		}

		PerformJump(CurrentJumpChargeRatio);

		_currentRoutine = null;
	}

	void PerformJump(in float ratio)
	{
		var jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, _jumpChargeCurve.Evaluate(ratio));
		_rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
		_currentJumpChargeSeconds = 0.0f;
	}

	Rigidbody _rigidbody;
	float CurrentJumpChargeRatio => Mathf.Clamp01(_currentJumpChargeSeconds / _maxJumpChargeSeconds);
	float _currentJumpChargeSeconds;
	[SerializeField] float _maxJumpChargeSeconds = 1.0f;
	[SerializeField] float _minJumpForce = 5.0f;
	[SerializeField] float _maxJumpForce = 10.0f;
	[SerializeField] AnimationCurve _jumpChargeCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
}

}
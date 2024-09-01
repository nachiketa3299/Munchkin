using System.Collections;

using UnityEngine;

namespace MC
{

/// <summary>
/// 시야 행동에 대해 캐릭터가 처리해야 할 일을 담당(애니메이션 등)
/// </summary>
[DisallowMultipleComponent]
public class LookAction : MonoBehaviour
{

#region UnityCallbacks
	void Start()
	{
		_initialTrackedObjectOffsetY = VirtualCamera.Instance.TrackedObjectOffsetY;
	}
#endregion

	public void BeginAction(float directionValue)
	{
		StopAllCoroutines();
		StartCoroutine(LookActionRoutine(directionValue));
	}

	public void EndAction()
	{
		StopAllCoroutines();
		StartCoroutine(LookActionRecoverRoutine());
	}

	IEnumerator LookActionRoutine(float direction)
	{
		var startOffsetY = VirtualCamera.Instance.TrackedObjectOffsetY;
		var targetOffsetY = direction * Mathf.Abs(_maxLookOffsetY);
		_lookElapsedTime = 0.0f;

		while (_lookElapsedTime < _lookDuration)
		{
			_lookElapsedTime += Time.deltaTime;
			var time = _blendingCurve.Evaluate(_lookElapsedTime / _lookDuration);

			VirtualCamera.Instance.TrackedObjectOffsetY = Mathf.Lerp(startOffsetY, targetOffsetY, time);
			yield return null;
		}

		_lookElapsedTime = 0.0f;
		VirtualCamera.Instance.TrackedObjectOffsetY = targetOffsetY;
	}

	IEnumerator LookActionRecoverRoutine()
	{
		var startOffsetY = VirtualCamera.Instance.TrackedObjectOffsetY;

		_recoverElapsedTime = 0.0f;

		while (_recoverElapsedTime < _recoverDuration)
		{
			_recoverElapsedTime += Time.deltaTime;
			var time = _blendingCurve.Evaluate(_recoverElapsedTime / _recoverDuration);

			VirtualCamera.Instance.TrackedObjectOffsetY = Mathf.Lerp(startOffsetY, _initialTrackedObjectOffsetY, time);
			yield return null;
		}
		_recoverElapsedTime = 0.0f;
		VirtualCamera.Instance.TrackedObjectOffsetY = _initialTrackedObjectOffsetY;

	}

	float _initialTrackedObjectOffsetY;
	[SerializeField] float _lookDuration = 0.1f;
	[SerializeField][HideInInspector] float _lookElapsedTime = 0.0f;
	[SerializeField] float _recoverDuration = 0.1f;
	[SerializeField][HideInInspector] float _recoverElapsedTime = 0.0f;
	[SerializeField] float _maxLookOffsetY = 4.0f;
	[SerializeField] AnimationCurve _blendingCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

}

}
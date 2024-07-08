using System.Collections;

using UnityEngine;

namespace MC
{

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class JumpAction : MonoBehaviour
	{
		/// <summary> 현재 실행 중인 점프 관련 코루틴을 모두 정지하고, 새로 점프 충전 코루틴을 시작한다. </summary>
		public void BeginAction()
		{
			TryStopJumpCoroutine();

			_currentRoutine = StartCoroutine(JumpChargeRoutine());
		}


		/// <summary> 현재 실행 중인 점프 관련 코루틴을 모두 정지하고, 계산된 점프 값으로 점프를 실행한다. </summary>
		public void EndAction()
		{
			TryStopJumpCoroutine();
			PerformJump();
		}

		#region Unity Messages

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		#endregion // Unity Messages

		#region Coroutines

		/// <summary> 점프 버튼이 떼어지기 전까지 점프를 누른 시간 동안 <see cref="_currentJumpChargeSeconds"/> 를 축적한다. </summary>
		IEnumerator JumpChargeRoutine()
		{
			_currentJumpChargeSeconds = 0.0f;
			while (CurrentJumpChargeRatio < 1.0f)
			{
				_currentJumpChargeSeconds += Time.deltaTime;
				yield return null;
			}

			PerformJump();

			_currentRoutine = null;

			yield return null;
		}

		#endregion // Coroutines

		/// <summary> 현재 실행중인 점프 관련 코루틴이 있다면 종료하고 null로 만든다. 없다면, 아무것도 하지 않는다. </summary>
		void TryStopJumpCoroutine()
		{
			if (_currentRoutine == null)
			{
				return;
			}

			StopCoroutine(_currentRoutine);
			_currentRoutine = null;
		}

		/// <summary> 점프를 수행한다. </summary>
		void PerformJump()
		{
			var jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, CurrentJumpChargeRatio);
			_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			_currentJumpChargeSeconds = 0.0f;
		}

		Rigidbody _rigidbody;
		Coroutine _currentRoutine;

		float CurrentJumpChargeRatio => Mathf.Clamp01(_currentJumpChargeSeconds / _maxJumpChargeSeconds);
		float _currentJumpChargeSeconds;

		[SerializeField] float _maxJumpChargeSeconds = 1.0f;
		[SerializeField] float _minJumpForce = 5.0f;
		[SerializeField] float _maxJumpForce = 10.0f;
	}
} // namespace
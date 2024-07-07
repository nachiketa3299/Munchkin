using System.Collections;
using UnityEngine;

namespace MC
{

	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class JumpAction : MonoBehaviour
	{
		public void BeginAction()
		{
			if (_currentRoutine != null)
			{
				return;
			}

			_currentRoutine = StartCoroutine(JumpChargeRoutine());
		}

		public void EndAction()
		{
			if (_currentRoutine != null)
			{
				StopCoroutine(_currentRoutine);
				_currentRoutine = null;
				PerformJump();
			}
		}

		#region Unity Messages

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		#endregion // Unity Messages

		#region Coroutines

		IEnumerator JumpChargeRoutine()
		{
			var chargeTime = 0.0f;
			while (chargeTime < _maxJumpChargeTime)
			{
				chargeTime += Time.deltaTime;
				_currentChargeRatio = Mathf.Clamp01(chargeTime / _maxJumpChargeTime);
				yield return null;
			}

			PerformJump();
			_currentRoutine = null;

			yield return null;
		}

		#endregion // Coroutines

		void PerformJump()
		{
			var jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, _currentChargeRatio);
			_rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			_currentChargeRatio = 0.0f;
		}


		Rigidbody _rigidbody;
		Coroutine _currentRoutine;
		float _currentChargeRatio;

		[SerializeField] float _maxJumpChargeTime = 1.0f;
		[SerializeField] float _minJumpForce = 5.0f;
		[SerializeField] float _maxJumpForce = 10.0f;
	}
} // namespace 
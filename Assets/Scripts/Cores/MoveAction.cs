using System;
using System.Collections;

using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class MoveAction : MonoBehaviour
	{
		public void BeginAction(float directionCoeff)
		{
			_directionCoeff = directionCoeff;

			TryStopMoveCouroutine();
			_currentRoutine = StartCoroutine(HorizontalAccelerationRoutine());
		}

		public void EndAction()
		{
			TryStopMoveCouroutine();
			_currentRoutine = StartCoroutine(HorizontalDeclerationRoutine());
		}

		#region Unity Messages

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		#endregion // Unity Messages

		#region Coroutines

		IEnumerator HorizontalAccelerationRoutine()
		{
			while (true)
			{
				if (Mathf.Abs(_rigidbody.velocity.x) < _maxHorizontalSpeed)
				{
					var force = _directionCoeff * _accelerationMagnitude * Vector3.right;
					_rigidbody.AddForce(force, ForceMode.Acceleration);
				}

				yield return new WaitForFixedUpdate();
			}
		}

		IEnumerator HorizontalDeclerationRoutine()
		{
			while (Mathf.Abs(_rigidbody.velocity.x) > 0.01f)
			{
				var force = -1.0f * _decelerationMagnitude * Mathf.Sign(_rigidbody.velocity.x) * Vector3.right;
				_rigidbody.AddForce(force, ForceMode.Acceleration);

				yield return new WaitForFixedUpdate();
			}
		}

		#endregion // Coroutines

		void TryStopMoveCouroutine()
		{
			if (_currentRoutine == null)
			{
				return;
			}

			StopCoroutine(_currentRoutine);
			_currentRoutine = null;
		}

		Rigidbody _rigidbody;

		float _directionCoeff;
		Coroutine _currentRoutine;

		[SerializeField] float _maxHorizontalSpeed = 10.0f;
		[SerializeField][Range(1.0f, 100.0f)] float _accelerationMagnitude = 10.0f;
		[SerializeField][Range(1.0f, 100.0f)] float _decelerationMagnitude = 10.0f;
	}
}

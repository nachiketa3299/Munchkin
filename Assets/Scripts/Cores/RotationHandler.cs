using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MC
{
	/// <summary>
	/// 캐릭터의 향하는 좌/우 방향을 결정하는 컴포넌트
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class RotationHandler : ActionRoutineBase
	{
		[Serializable] private enum EDirection { Left, Right }

		#region Unity Message

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		void OnEnable()
		{
			_rigidbody.MoveRotation(Quaternion.Euler(0.0f, _directionToEulerAngleY[_initialDirection], 0.0f));
			_lastDirection = _initialDirection;
		}

		/// <summary>
		/// 수평 속도가 0 미만이면 왼쪽을 바라보고 있어야 하고, 0 초과면 오른쪽을 바라보고 있어야 하며, 아닌 경우 방향을 유지.
		/// </summary>
		/// <remarks>
		/// 단, 기준을 0으로 하면 작은 수평 속도에서 캐릭터가 계속 Flip 하는 문제가 있어서, Threshold를 준다.
		/// </remarks>
		void FixedUpdate()
		{
			if (HorizontalVelocity < -1.0f * Mathf.Abs(_directionChangeHorizontalVelocityThreshold))
			{
				if (_lastDirection == EDirection.Right)
				{
					TryStopCurrentRoutine();
					_currentRoutine = StartCoroutine(RotateRoutine(EDirection.Left));

					_lastDirection = EDirection.Left;
				}
			}

			else if (HorizontalVelocity > Mathf.Abs(_directionChangeHorizontalVelocityThreshold))
			{
				if (_lastDirection == EDirection.Left)
				{
					TryStopCurrentRoutine();
					_currentRoutine = StartCoroutine(RotateRoutine(EDirection.Right));

					_lastDirection = EDirection.Right;
				}
			}
		}

		#endregion // Unity Message

		/// <summary>
		//  (왼쪽 -> 오른쪽) 일때 반시계방향, (오른쪽 -> 왼쪽)일때 시계방향으로 돌리기 위해, 타겟 앵글을 미세하게 조정해준다.
		/// </summary>
		float CalibrateAngle(EDirection to, float angle)
		{
			switch (to)
			{
				case EDirection.Left:
					angle -= 0.2f;
					break;
				case EDirection.Right:
					angle += 0.2f;
					break;
			}

			return angle;
		}

		IEnumerator RotateRoutine(EDirection to)
		{
			var elapsedTime = 0.0f;
			var initialRotation = transform.rotation;

			var targetYAngle = _directionToEulerAngleY[to];
			var calibratedTargetYAngle = CalibrateAngle(to, targetYAngle);

			var targetRotation = Quaternion.Euler(0.0f, targetYAngle, 0.0f);
			var calibratedTargetRotation = Quaternion.Euler(0.0f, calibratedTargetYAngle, 0.0f);

			while (elapsedTime < _rotationTime)
			{
				var curveValue = _rotationCurve.Evaluate(elapsedTime / _rotationTime);
				var currentRot = Quaternion.Lerp(initialRotation, calibratedTargetRotation, curveValue);

				transform.rotation = currentRot;

				elapsedTime += Time.deltaTime;

				yield return null;
			}

			transform.rotation = targetRotation;
		}

		float HorizontalVelocity => _rigidbody.velocity.x;

		readonly Dictionary<EDirection, float> _directionToEulerAngleY = new() { { EDirection.Left, -90.0f }, { EDirection.Right, 90.0f } };

		Rigidbody _rigidbody;
		EDirection _lastDirection;

		[SerializeField] EDirection _initialDirection = EDirection.Right;
		[SerializeField] float _rotationTime = 0.1f;
		[SerializeField] AnimationCurve _rotationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
		[SerializeField] float _directionChangeHorizontalVelocityThreshold = 0.5f;

	} // class
} // namespace
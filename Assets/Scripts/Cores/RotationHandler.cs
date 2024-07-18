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
		/// <summary>
		/// 캐릭터가 바라보는 방향에 대한 열거형
		/// </summary>
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
		/// 캐릭터의 Rigidbody 수평 속도를 체크하여 회전이 일어나야 하는 시점에 회전 코루틴을 호출한다.
		/// </summary>
		/// <remarks>
		/// 단, 수평 속도 기준을 0으로 하면 작은 수평 속도에서 캐릭터가 계속 Flip 하는 문제가 있어서, Threshold를 준다.
		/// </remarks>
		void FixedUpdate()
		{
			if (HorizontalRigidbodyVelocity < -1.0f * Mathf.Abs(_directionChangeHorizontalVelocityThreshold))
			{
				if (_lastDirection == EDirection.Right)
				{
					TryStopCurrentRoutine();

					var rotationRoutine = _lerpRotation ? LerpRotateRoutine(EDirection.Left) : FlipRotateRoutine(EDirection.Left);
					_currentRoutine = StartCoroutine(rotationRoutine);

					_lastDirection = EDirection.Left;
				}
			}

			else if (HorizontalRigidbodyVelocity > Mathf.Abs(_directionChangeHorizontalVelocityThreshold))
			{
				if (_lastDirection == EDirection.Left)
				{
					TryStopCurrentRoutine();

					var rotationRoutine = _lerpRotation ? LerpRotateRoutine(EDirection.Right) : FlipRotateRoutine(EDirection.Right);
					_currentRoutine = StartCoroutine(rotationRoutine);

					_lastDirection = EDirection.Right;
				}
			}
		}

		#endregion // Unity Message

		/// <summary>
		///  (왼쪽 -> 오른쪽) 일때 반시계방향, (오른쪽 -> 왼쪽)일때 시계방향으로 돌리기 위해, 타겟 앵글을 미세하게 조정해준다.
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

		/// <summary>
		/// 캐릭터를 Rigidbody 를 이용하여 <paramref name="to"/> 방향까지 속도에 맞게 보간하여 회전시킨다.
		/// </summary>
		IEnumerator LerpRotateRoutine(EDirection to)
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
				var currentRotation = Quaternion.Lerp(initialRotation, calibratedTargetRotation, curveValue);

				_rigidbody.MoveRotation(currentRotation);

				elapsedTime += Time.deltaTime;

				yield return null;
			}

			_rigidbody.MoveRotation(targetRotation);
		}

		/// <summary>
		/// 캐릭터를 Rigidbody를 이용하여 <paramref name="to"/> 방향까지 즉각적으로 회전시킨다.
		/// </summary>
		IEnumerator FlipRotateRoutine(EDirection to)
		{
			var targetRotation = Quaternion.Euler(0.0f, _directionToEulerAngleY[to], 0.0f);
			_rigidbody.MoveRotation(targetRotation);
			yield break;
		}

		float HorizontalRigidbodyVelocity => _rigidbody.velocity.x;

		readonly Dictionary<EDirection, float> _directionToEulerAngleY = new() { { EDirection.Left, -90.0f }, { EDirection.Right, 90.0f } };

		Rigidbody _rigidbody;
		EDirection _lastDirection;

		[SerializeField] EDirection _initialDirection = EDirection.Right;
		[SerializeField] bool _lerpRotation = true;

		[Header("Lerp Rotation 설정")]

		[SerializeField] float _rotationTime = 0.1f;
		[SerializeField] AnimationCurve _rotationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
		[SerializeField] float _directionChangeHorizontalVelocityThreshold = 0.5f;

	} // class
} // namespace
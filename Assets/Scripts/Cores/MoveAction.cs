using System;
using System.Collections;

using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class MoveAction : MonoBehaviour
	{
		/// <summary> 현재 실행중인 운동 관련 코루틴을 모두 정지하고, 새로 <paramref name="directionCoeff"/> 방향으로 가속하는 코루틴을 실행한다.</summary>
		public void BeginAction(float directionCoeff)
		{
			_directionCoeff = directionCoeff;

			TryStopMoveCouroutine();

			_currentRoutine = StartCoroutine(HorizontalAccelerationRoutine());
		}

		/// <summary> 현재 실행중인 운동 관련 코루틴을 모두 정지하고, 감속 코루틴을 시작한다. </summary>
		public void EndAction()
		{
			TryStopMoveCouroutine();
			_currentRoutine = StartCoroutine(HorizontalDecelerationRoutine());
		}

		#region Unity Messages

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		#endregion // Unity Messages

		#region Coroutines

		/// <summary> 캐릭터를 수평 방향으로 설정된 가속도로 가속하고, 최대 수평 속력에 도달하면 그 속도를 유지하는 코루틴. </summary>
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

		/// <summary> 이동 입력이 존재하지 않을 때 캐릭터를 설정된 감속도로 수평 속력이 0이 될 때가지 감속하는 코루틴. </summary>
		IEnumerator HorizontalDecelerationRoutine()
		{
			while (Mathf.Abs(_rigidbody.velocity.x) > 0.01f)
			{
				var force = -1.0f * _decelerationMagnitude * Mathf.Sign(_rigidbody.velocity.x) * Vector3.right;
				_rigidbody.AddForce(force, ForceMode.Acceleration);

				yield return new WaitForFixedUpdate();
			}
		}

		#endregion // Coroutines

		/// <summary> 현재 실행중인 이동 관련 코루틴이 있다면 종료하고 null로 만든다. 없다면, 아무것도 하지 않는다. </summary>
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

		// TODO 지면에 있는 경우 / 체공해 있는 경우 수평 가/감속도가 달라야 함.
		// TODO 또한, 캐릭터의 종류별로 아래 멤버의 값이 달라야 함. 캐릭터 오브젝트가 커지는 경우 각 멤버를 캐릭터별로 어떻게 저장하고 초기화할 지 고민할 것.
		[SerializeField] float _maxHorizontalSpeed = 10.0f;
		[SerializeField][Range(1.0f, 100.0f)] float _accelerationMagnitude = 10.0f;
		[SerializeField][Range(1.0f, 100.0f)] float _decelerationMagnitude = 10.0f;
	}
}

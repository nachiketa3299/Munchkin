using System.Collections;

using Cinemachine;

using UnityEngine;
using UnityEngine.Assertions;


namespace MC
{
	/// <summary>
	/// 시야 행동에 대해 카메라가 처리해야 할 일을 담당.
	/// </summary>
	/// <remarks>
	/// 여기서는 카메라 타겟의 Y 오프셋을 설정하되, Blending 방식은 시네머신 자체에서 처리하도록 한다.
	/// </remarks>
	[RequireComponent(typeof(CinemachineVirtualCamera))]
	public class CameraLookAction : ActionRoutineBase
	{
		#region Unity Messages

		void Awake()
		{
			_virtualCamera = GetComponent<CinemachineVirtualCamera>();
			_framingTransposer = _virtualCamera?.GetCinemachineComponent<CinemachineFramingTransposer>();

#if UNITY_EDITOR
			Assert.IsNotNull(_virtualCamera);
			Assert.IsNotNull(_framingTransposer);
#endif
		}

		#endregion // Unity Messages

		/// <summary>
		/// 현재 실행 중인 카메라 시야 관련 코루틴을 모두 정지하고,
		/// 새로 <paramref name="directionCoeff"/> 방향으로 카메라 Y 오프셋을 이동시키는 코루틴을 실행한다.
		/// </summary>
		public void BeginAction(float directionCoeff)
		{
			TryStopCurrentRoutine();

			_currentRoutine = StartCoroutine(CameraLookRoutine(directionCoeff));
		}

		/// <summary>
		/// 현재 실행 중인 카메라 시야 관련 코루틴을 모두 정지하고, 카메라 시야 회복 코루틴을 시작한다.
		/// </summary>
		public void EndAction()
		{
			TryStopCurrentRoutine();

			_currentRoutine = StartCoroutine(CameraRecoverRoutine());
		}


		/// <summary>
		/// 카메라의 객체 추적 Y 오프셋을 <paramref name="directionCoeff"/> 방향으로 <see cref="_maxCameraLookOffsetY"/> 까지 점진적으로 이동시키는 코루틴.
		/// </summary>
		IEnumerator CameraLookRoutine(float directionCoeff)
		{
			_currentCameraLookOffsetY = _framingTransposer.m_TrackedObjectOffset.y;

			var targetCameraLookOffsetY = directionCoeff * Mathf.Abs(_maxCameraLookOffsetY);

			while (Mathf.Abs(_currentCameraLookOffsetY - targetCameraLookOffsetY) > 0.01f)
			{
				_currentCameraLookOffsetY = Mathf.MoveTowards(_currentCameraLookOffsetY, targetCameraLookOffsetY, _lookSpeed * Time.deltaTime);
				UpdateCameraOffsetY();

				yield return null;
			}

			_currentCameraLookOffsetY = targetCameraLookOffsetY;
			UpdateCameraOffsetY();
		}

		/// <summary>
		/// 카메라의 객체 추적 Y 오프셋을 0으로 점진적으로 이동시키는 코루틴
		/// </summary>
		IEnumerator CameraRecoverRoutine()
		{
			while (Mathf.Abs(_currentCameraLookOffsetY) > 0.01f)
			{
				_currentCameraLookOffsetY = Mathf.MoveTowards(_currentCameraLookOffsetY, 0.0f, _recoverSpeed * Time.deltaTime);
				UpdateCameraOffsetY();
				yield return null;
			}

			_currentCameraLookOffsetY = 0.0f;
			UpdateCameraOffsetY();
		}

		void UpdateCameraOffsetY() => _framingTransposer.m_TrackedObjectOffset.y = _currentCameraLookOffsetY;

		CinemachineVirtualCamera _virtualCamera;
		CinemachineFramingTransposer _framingTransposer;

		float _currentCameraLookOffsetY = 0.0f;

		[SerializeField] float _maxCameraLookOffsetY = 2.5f;
		[SerializeField] float _recoverSpeed = 3.0f;
		[SerializeField] float _lookSpeed = 3.0f;
	}
} // namespace
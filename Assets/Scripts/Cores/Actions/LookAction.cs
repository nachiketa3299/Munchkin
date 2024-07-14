using UnityEngine;

namespace MC
{
	/// <summary> 시야 행동에 대해 캐릭터가 처리해야 할 일을 담당(애니메이션 등) </summary>
	/// <remarks> 카메라가 이동은 <see cref="CameraLookAction"/> 에서 처리 </remarks>
	[DisallowMultipleComponent]
	public class LookAction : MonoBehaviour
	{
		public void BeginAction(float directionCoeff)
		{
			// 여기에 캐릭터의 시야행동 시작 로직을 처리

			_cameraLookAction?.BeginAction(directionCoeff);
		}

		public void EndAction()
		{
			// 여기에 캐릭터의 시야행동 끝 로직을 처리

			_cameraLookAction?.EndAction();
		}

		#region Unity Messages

		void Awake()
		{
#if UNITY_EDITOR
			if (!_cameraLookAction)
			{
				Debug.LogWarning("가상 카메라 오브젝트가 LookAction 인스펙터에 할당되지 않았습니다.");
			}
#endif
		}

		#endregion

		// NOTE 무슨 매니저 이런거 만들기 싫어서 일단 에디터에서 연결하는 것으로 결정, 추후에 필요성이 생기는 경우 따로 연결 방법 고심.
		[SerializeField] CameraLookAction _cameraLookAction;
	}
} // namespace
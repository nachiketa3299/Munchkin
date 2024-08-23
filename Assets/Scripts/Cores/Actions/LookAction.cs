using UnityEngine;

namespace MC
{

/// <summary>
/// 시야 행동에 대해 캐릭터가 처리해야 할 일을 담당(애니메이션 등)
/// </summary>
[DisallowMultipleComponent]
public class LookAction : MonoBehaviour
{
	public void BeginAction(float directionValue) => CameraLookAction.Instance.BeginAction(directionValue);

	public void EndAction() => CameraLookAction.Instance.EndAction();
}

}
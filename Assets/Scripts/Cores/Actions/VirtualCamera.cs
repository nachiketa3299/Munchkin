using System.Collections;

using Cinemachine;

using UnityEngine;

namespace MC
{

/// <summary>
/// 캐릭터의 카메라를 조작
/// </summary>
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VirtualCamera : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{
		Instance = this;

		_virtualCamera = GetComponent<CinemachineVirtualCamera>();
		_framingTransposer = _virtualCamera?.GetCinemachineComponent<CinemachineFramingTransposer>();

#if UNITY_EDITOR
		if (!_virtualCamera)
		{
			Debug.Log("Cinemachine Virtual Camera를 찾을 수 없습니다.");
		}
		if (!_framingTransposer)
		{
			Debug.Log("가상 카메라에서 Cinemachine Framing Transposer를 찾을 수 없습니다.");
		}
#endif
	}

#endregion // UnityCallbacks

	/// <remarks>
	/// 현재 <see cref="LookAction">과 엄청난 커플링이 존재, 그러나 그냥 작동하므로 사용
	/// </remarks>
	public static VirtualCamera Instance { get; private set; }

	public float TrackedObjectOffsetY
	{
		get => _framingTransposer.m_TrackedObjectOffset.y;
		set => _framingTransposer.m_TrackedObjectOffset.y = value;
	}

	CinemachineVirtualCamera _virtualCamera;
	CinemachineFramingTransposer _framingTransposer;

}

}
#if UNITY_EDITOR

using UnityEngine;

namespace MC
{
	/// <summary>
	/// 캐릭터 선택시 캐릭터 전방 표시
	/// </summary>
	public class MovementGizmos : MonoBehaviour
	{
		void OnDrawGizmos()
		{
			Gizmos.color = _lineColor;
			Gizmos.DrawRay(transform.position, transform.forward * _lineLength);
		}

		[SerializeField] float _lineLength = 1f;
		[SerializeField] Color _lineColor = Color.red;
	}
}

#endif
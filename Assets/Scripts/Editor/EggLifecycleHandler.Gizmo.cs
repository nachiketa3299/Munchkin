#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC
{
	public partial class EggLifecycleHandler : MonoBehaviour
	{
		/// <summary>
		/// <paramref name="owner"/>가 매 생애주기 마다 바뀌므로, 팩토리나 풀이 아닌, 여기에서 주기가 시작될 때마다 이름을 재설정 해주어야 함.
		/// </summary>
		string MakeInstanceName(in EEggOwner owner) => $"{owner}Egg ({gameObject.GetInstanceID()})";

		[DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
		static void DrawUniqueNameOfEgg(EggLifecycleHandler target, GizmoType gizmoType)
		{
			var style = new GUIStyle();
			{
				style.normal.textColor = Color.yellow;
				style.alignment = TextAnchor.MiddleCenter;
			}

			Handles.Label
			(
				position: target.transform.position,
				text: target.gameObject.name,
				style: style
			);
		}
	}
}

#endif
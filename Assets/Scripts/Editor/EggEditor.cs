#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC
{
	public partial class Egg : MonoBehaviour
	{

		[CustomEditor(typeof(Egg))]
		private class EggEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				var egg = target as Egg;

				if (egg == null)
				{
					return;
				}

				EditorGUILayout.Space(10.0f);

				// Health Gage
				EditorGUILayout.LabelField("알 체력");
				var rect = GUILayoutUtility.GetRect(18.0f, 18.0f);

				EditorGUI.ProgressBar
				(
					rect,
					egg.HealthRatio,
					$"{egg._currentHealth:F2}/{egg._maxHealth:F2} ({egg.HealthRatio:P2})"
				);


				// Damage Timer
				EditorGUILayout.Space(10.0f);
				EditorGUILayout.LabelField("알 데미지 타이머");

				var canBeDamagedString = egg._eggCanBeDamaged ? "현재 알이 데미지를 받을 수 있는 상태입니다." : "<color='red'>현재 알이 데미지를 받을 수 없는 상태입니다.</color>";

				EditorGUILayout.LabelField(canBeDamagedString, new GUIStyle(EditorStyles.label) { richText = true });


				rect = GUILayoutUtility.GetRect(18.0f, 9.0f);

				EditorGUI.ProgressBar
				(
					rect,
					egg.EggDamageTimerRatio,
					$"{egg._eggDamageTimerElapsedTime:F3}s/{egg._eggDamageTimer:F3} ({egg.EggDamageTimerRatio:P2})"
				);

				// ImpactForce Progress Bar

				EditorGUILayout.Space(10.0f);
				EditorGUILayout.LabelField("충격력 게이지");

				rect = GUILayoutUtility.GetRect(18.0f, 9.0f);
				EditorGUI.ProgressBar
				(
					rect,
					egg.ImpactRatio,
					$"F: {egg._lastImpactForce:F3} T: {egg._impactThreshold:F3} ({egg.ImpactRatio})"
				);

				if (Application.isPlaying)
				{
					Repaint();
				}

			}
		}
	}
}

#endif

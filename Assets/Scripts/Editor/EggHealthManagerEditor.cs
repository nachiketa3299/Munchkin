#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC
{
		[CustomEditor(typeof(EggHealthManager))]
		public class EggHealthManagerEditor : Editor
		{

			#region UnityCallbacks

			public override void OnInspectorGUI()
			{
				serializedObject.Update();
				base.OnInspectorGUI();

				EditorGUILayout.Space();

				// Health gage
				EditorGUILayout.LabelField("Health Gage");

				var rect = EditorGUILayout.GetControlRect(false, 18.0f);
				var currentHealth = serializedObject.FindProperty("_currentHealth").floatValue;
				var maxHealth = serializedObject.FindProperty("_maxHealth").floatValue;
				var healthRatio = Mathf.Clamp01(currentHealth / maxHealth);
				var healthGageLabel = $"{currentHealth}/{maxHealth} ({healthRatio:P2})";

				EditorGUI.ProgressBar
				(
					position: rect,
					value: healthRatio,
					text: healthGageLabel
				);

				EditorGUILayout.Space();

				// Damageable label

				EditorGUILayout.LabelField("Egg damage timers:");

				var canBeDamaged = serializedObject.FindProperty("_canBeDamaged").boolValue;
				var damageableLabel = canBeDamaged
					? "<color='green'>Can be damaged</color>"
					: "<color='red'>Can't be damaged</color>";

				EditorGUILayout.LabelField
				(
					label: damageableLabel,
					style: new GUIStyle(EditorStyles.textArea) {richText = true}
				);

				// Damage block timer
				EditorGUILayout.LabelField("Egg damage timer for creation");

				rect = GUILayoutUtility.GetRect(18.0f, 9.0f);
				var elapsedTime =  serializedObject.FindProperty("_damageTimerElapsedTime").floatValue;
				var maxTimeForCreation = serializedObject.FindProperty("_currentDamageTimerMaxTime").floatValue;
				var creationTimerRatio = Mathf.Clamp01(elapsedTime / maxTimeForCreation);
				var creationTimerLabel = $"{elapsedTime}/{maxTimeForCreation} ({(float.IsNaN(creationTimerRatio) ? 0.0f:creationTimerRatio):P2})";

				EditorGUI.ProgressBar
				(
					position: rect,
					value: creationTimerRatio,
					text: creationTimerLabel
				);

				EditorGUILayout.Space();

				// Utility Tools
				if (EditorApplication.isPlaying)
				{
					_damageToForceInflict = EditorGUILayout.FloatField("Damage to force inflict: ", _damageToForceInflict);

					if (GUILayout.Button("Force inflict damage"))
					{
						var eggHealthManager = target as EggHealthManager;

						if (!eggHealthManager.gameObject.activeSelf)
						{
							Debug.Log("활성화되지 않은 알의 체력을 감소시킬 수 없습니다.");
						}
						else
						{
							eggHealthManager.InflictDamage(Mathf.Abs(_damageToForceInflict));
						}
					}
				}
				serializedObject.ApplyModifiedProperties();
			}

			#endregion // UnityCallbacks
			float _damageToForceInflict;
		}
} // namespace
#endif
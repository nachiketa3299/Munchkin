#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
namespace MC
{

	public partial class EggHealthManager : MonoBehaviour
	{
		float DamageBlockTimerProgress => Mathf.Clamp01(_damageTimerElapsedTime / _damageTimerMaxTime);

		[CustomEditor(typeof(EggHealthManager))]
		private class EggHealthManagerEditor : Editor
		{
			EggHealthManager _eggHealthManager;
			GUIStyle _labelStyle;

			#region UnityCallbacks

			void OnEnable()
			{
				_eggHealthManager = target as EggHealthManager;
				_labelStyle = new(EditorStyles.label) { richText = true }; // TODO 이렇게 하는게 맞는지 모르겠음.
			}

			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				if (!_eggHealthManager)
				{
					return;
				}

				EditorGUILayout.Space();

				// Health Gage
				EditorGUILayout.LabelField("Egg Health Gage");

				var rect = GUILayoutUtility.GetRect(18.0f, 18.0f);
				var ratio = _eggHealthManager.HealthRatio;
				var ratioString = $"{_eggHealthManager._currentHealth}/{_eggHealthManager._maxHealth} ({ratio:P2})";

				EditorGUI.ProgressBar
				(
					position: rect,
					value: ratio,
					text: ratioString
				);

				// Damage Block Timer
				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Egg Damage Timer");

				var canBeDamagedString = _eggHealthManager._canBeDamaged
					? "Egg can be damaged"
					: "<color='red'>Egg can't be damaged</color>";

				EditorGUILayout.LabelField
				(
					label: canBeDamagedString,
					style: _labelStyle
				);

				rect = GUILayoutUtility.GetRect(18.0f, 9.0f);
				ratio = _eggHealthManager.DamageBlockTimerProgress;
				ratioString = $"{_eggHealthManager._damageTimerElapsedTime}/{_eggHealthManager._damageTimerMaxTime} ({ratio:P2})";

				EditorGUI.ProgressBar
				(
					position: rect,
					value: ratio,
					text: ratioString
				);

				if (Application.isPlaying)
				{
					Repaint();
				}
			}

			#endregion
		} // inner class
	} // outer class
} // namespace

#endif
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

	[CustomEditor(typeof(LookAction))]
internal sealed class LookActionEditor : Editor
{

#region UnityCallbacks

		public override bool RequiresConstantRepaint() => true;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			serializedObject.Update();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Look Action Timer", EditorStyles.boldLabel);

			{
				var rect = EditorGUILayout.GetControlRect(false, 9.0f);
				var elapsed = serializedObject.FindProperty("_lookElapsedTime").floatValue;
				var max = serializedObject.FindProperty("_lookDuration").floatValue;
				var ratio = Mathf.Clamp01(elapsed / max);
				ratio = float.IsNaN(ratio) ? 0.0f : ratio;

				var label = $"{elapsed:F2}/{max:F2} ({ratio:P2})";
				EditorGUI.ProgressBar
				(
					position: rect,
					value: ratio,
					text: label
				);
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Look Action Recover Timer", EditorStyles.boldLabel);
			{
				var rect = EditorGUILayout.GetControlRect(false, 9.0f);
				var elapsed = serializedObject.FindProperty("_recoverElapsedTime").floatValue;
				var max = serializedObject.FindProperty("_recoverDuration").floatValue;
				var ratio = Mathf.Clamp01(elapsed / max);
				ratio = float.IsNaN(ratio) ? 0.0f : ratio;

				var label = $"{elapsed:F2}/{max:F2} ({ratio:P2})";
				EditorGUI.ProgressBar
				(
					position: rect,
					value: ratio,
					text: label
				);
			}

			serializedObject.ApplyModifiedProperties();
		}

#endregion // UnityCallbacks

	}

}

#endif
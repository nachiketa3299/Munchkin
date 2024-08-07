#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(EggAction))]
internal sealed class EggActionEditor : Editor
{
	#region UnityCallbacks

	public override bool RequiresConstantRepaint() => true;

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Egg action charge gage");

		var rect = EditorGUILayout.GetControlRect(false, 18.0f);
		var currentCharge = serializedObject.FindProperty("_eggActionChargeTimeCurrent").floatValue;
		var maxCharge = serializedObject.FindProperty("_eggActionChargeTimeMax").floatValue;
		var ratio = Mathf.Clamp01(currentCharge / maxCharge);
		ratio = float.IsNaN(ratio) ? 0.0f : ratio;

		var label = $"{currentCharge:F2}/{maxCharge:F2} ({ratio:P2})";

		EditorGUI.ProgressBar
		(
			position: rect,
			value: ratio,
			text: label
		);

		serializedObject.ApplyModifiedProperties();
	}

	#endregion // UnityCallbacks

	}
}

#endif
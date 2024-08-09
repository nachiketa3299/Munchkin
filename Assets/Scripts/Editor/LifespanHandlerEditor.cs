#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(LifespanHandler))]
internal sealed class LifespanHandlerEditor : Editor
{

#region UnityCallbacks

	public override bool RequiresConstantRepaint() => true;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (!target)
		{
			return;
		}

		serializedObject.Update();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Lifespan progress");

		var rect = GUILayoutUtility.GetRect(18.0f, 18.0f);

		var currentLifespan = serializedObject.FindProperty("_currentLifespan").floatValue;
		var maxLifespan = serializedObject.FindProperty("_maxLifespan").floatValue;
		var ratio = Mathf.Clamp01(currentLifespan / maxLifespan);
		ratio = float.IsNaN(ratio) ? 0.0f : ratio;
		var label = $"{currentLifespan:F2}/{maxLifespan:F2} ({ratio:P2})";

		EditorGUI.ProgressBar
		(
			position: rect,
			value: ratio,
			text: label
		);

		var mutationThreshold = serializedObject.FindProperty("_mutationThreshold").floatValue;
		var thresholdXPos = rect.x + rect.width * (mutationThreshold / maxLifespan);

		EditorGUI.DrawRect(new Rect(thresholdXPos, rect.y, 1, rect.height), _mutationThresholdColor);

		serializedObject.ApplyModifiedProperties();
	}

	readonly Color _mutationThresholdColor = new(1.0f, 0.0f, 0.0f, 0.7f);
}

#endregion // UnityCallbacks

}

#endif
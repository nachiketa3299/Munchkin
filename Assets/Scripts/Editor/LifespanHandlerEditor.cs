#if UNITY_EDITOR

using System;
using System.Reflection;

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
		// Draw Progress bar of lifespan
		base.OnInspectorGUI();

		if (!target)
		{
			return;
		}

		serializedObject.Update();

		EditorGUILayout.Space();
		var lifespanHandler = target as LifespanHandler;

		// Lifespan progress bar
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

		// Mark mutation threshold

		var mutationThreshold = serializedObject.FindProperty("_mutationThreshold").floatValue;
		var thresholdXPos = rect.x + rect.width * (mutationThreshold / maxLifespan);

		EditorGUI.DrawRect(new Rect(thresholdXPos, rect.y, 1, rect.height), _mutationThresholdColor);

		// Buttons for lifespan control

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();

		var lifespanState = (LifespanHandler.ELifespanState) serializedObject.FindProperty("_currentState").enumValueIndex;
		var enabled = EditorApplication.isPlaying;

		GUI.enabled = enabled;

		if (GUILayout.Button("Restart lifespan"))
		{
			lifespanHandler.SendMessage("RestartLifespan");
		}

		GUI.enabled = enabled && lifespanState == LifespanHandler.ELifespanState.Paused;

		if (GUILayout.Button("Resume lifespan"))
		{
			lifespanHandler.SendMessage("ResumeLifespan");
		}

		GUI.enabled = enabled && lifespanState == LifespanHandler.ELifespanState.Running;

		if (GUILayout.Button("Pause lifespan"))
		{
			lifespanHandler.SendMessage("PauseLifespan");
		}

		GUI.enabled = enabled && lifespanState != LifespanHandler.ELifespanState.Ended;

		if (GUILayout.Button("End lifespan"))
		{
			lifespanHandler.SendMessage("EndLifespan");
		}

		GUI.enabled = true;

		EditorGUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}

	readonly Color _mutationThresholdColor = new(1.0f, 0.0f, 0.0f, 0.7f);
}

#endregion // UnityCallbacks

}

#endif
#if UNITY_EDITOR

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace MC.Editors
{

[CustomEditor(typeof(EggImpactDetector))]
internal sealed class EggImpactDetectorEditor : Editor
{
		#region UnityCallbacks

		public override bool RequiresConstantRepaint() => true;

		void OnEnable()
	{
		_thresholdLabelStyle.normal.textColor = _thresholdColor;
		_thresholdLabelStyle.fontSize = 10;

		_dataPointLabelStyle.normal.textColor = Color.white;
		_dataPointLabelStyle.fontSize = 10;

		_eggImpactDetector = target as EggImpactDetector;

		if (!_eggImpactDetector)
		{
			return;
		}

		_eggImpactDetector.Impacted += OnEggImpacted;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Impact Force Graph");

		EditorGUILayout.Space();

		var graphWidth = EditorGUIUtility.currentViewWidth - 40.0f;
		var graphHeight = 100.0f;

		var rect = EditorGUILayout.GetControlRect(false, graphHeight);
		rect.width = graphWidth;
		rect.x = (EditorGUIUtility.currentViewWidth - graphWidth) / 2;

		Handles.DrawSolidRectangleWithOutline(rect, _graphBackgroundColor, _graphBackgroundColor);

		var impactMagnitudeThreshold = serializedObject.FindProperty("_impactMagnitudeThreshold").floatValue;

		var maxForce =
		Mathf.Max
		(
			Mathf.Max(_impactMagnitudeCache.ToArray()),
			impactMagnitudeThreshold
		) * 1.4f;

		if (_impactMagnitudeCache.Count > 1)
		{
			for (var i = 0; i < _impactMagnitudeCache.Count - 1; ++i)
			{
				var start = new Vector2
				(
					x: rect.x + (i * rect.width / (_impactMagnitudeCache.Count - 1)),
					y: rect.y + rect.height - (_impactMagnitudeCache[i] / maxForce * rect.height)
				);

				var end = new Vector2
				(
					x: rect.x + ((i + 1) * rect.width / (_impactMagnitudeCache.Count - 1)),
					y: rect.y + rect.height - (_impactMagnitudeCache[i + 1] / maxForce * rect.height)
				);

				Handles.color = _dataPointColor;
				Handles.DrawLine
				(
					p1: start,
					p2: end,
					thickness: 0.0f
				);
				var labelPivot = new Vector2(end.x, end.y - 15f);
				Handles.Label
				(
					labelPivot,
					text: $"{_impactMagnitudeCache[i + 1]:F2}",
					style: _dataPointLabelStyle
				);
			}
		}

		var thresholdY = rect.y + rect.height - (impactMagnitudeThreshold);
		Handles.color = _thresholdColor;
		Handles.DrawLine
		(
			p1: new Vector2(rect.x, thresholdY),
			p2: new Vector2(rect.x + rect.width, thresholdY),
			thickness: 0.0f
		);

		Handles.Label
		(
			position: new Vector2(rect.x, thresholdY - 15.0f),
			text: $"Threshold: {impactMagnitudeThreshold:F2}",
			style: _thresholdLabelStyle
		);

		serializedObject.ApplyModifiedProperties();
	}

	void OnDisable()
	{
		if (!_eggImpactDetector)
		{
			return;
		}

		_eggImpactDetector.Impacted -= OnEggImpacted;
	}

	#endregion

	void OnEggImpacted(in Vector3 impact)
	{
		var impactMagnitude = impact.magnitude;
		_impactMagnitudeCache.Add(impactMagnitude);

		if (_impactMagnitudeCache.Count > MAX_DATA_POINTS)
		{
			_impactMagnitudeCache.RemoveAt(0);
		}
	}

	EggImpactDetector _eggImpactDetector;
	Color _graphBackgroundColor = new(0.15f, 0.15f, 0.15f, 1.0f);
	GUIStyle _thresholdLabelStyle = new();
	Color _thresholdColor = Color.yellow;
	GUIStyle _dataPointLabelStyle = new();
	Color _dataPointColor = Color.green;
	const int MAX_DATA_POINTS = 20;
	List<float> _impactMagnitudeCache = new();
}

}

#endif
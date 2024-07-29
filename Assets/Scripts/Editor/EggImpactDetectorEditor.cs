#if UNITY_EDITOR

using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace MC
{
	public partial class EggImpactDetector : MonoBehaviour
	{
		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawRay(transform.position, _lastImpactForce);
		}

		Vector3 _lastImpactForce;

		[CustomEditor(typeof(EggImpactDetector))]
		private class EggImpactDetectorEditor : Editor
		{
			#region Unity Callbacks

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

				_eggImpactDetector.EggImpacted += OnEggImpacted;
			}

			public override void OnInspectorGUI()
			{
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

				var maxForce =
				Mathf.Max
				(
					Mathf.Max(_impactForceMagnitudeCache.ToArray()),
					_eggImpactDetector._impactForceMagnitudeThreshold
				) * 1.4f;

				if (_impactForceMagnitudeCache.Count > 1)
				{
					for (var i = 0; i < _impactForceMagnitudeCache.Count - 1; ++i)
					{
						var start = new Vector2
						(
							x: rect.x + (i * rect.width / (_impactForceMagnitudeCache.Count - 1)),
							y: rect.y + rect.height - (_impactForceMagnitudeCache[i] / maxForce * rect.height)
						);

						var end = new Vector2
						(
							x: rect.x + ((i + 1) * rect.width / (_impactForceMagnitudeCache.Count - 1)),
							y: rect.y + rect.height - (_impactForceMagnitudeCache[i + 1] / maxForce * rect.height)
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
							text: $"{_impactForceMagnitudeCache[i + 1]:F2}",
							style: _dataPointLabelStyle
						);
					}
				}

				var thresholdY = rect.y + rect.height - (_eggImpactDetector._impactForceMagnitudeThreshold / maxForce * rect.height);
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
					text: $"Threshold: {_eggImpactDetector._impactForceMagnitudeThreshold:F2}",
					style: _thresholdLabelStyle
				);
			}

			void OnDisable()
			{
				if (!_eggImpactDetector)
				{
					return;
				}

				_eggImpactDetector.EggImpacted -= OnEggImpacted;
			}

			#endregion

			void OnEggImpacted(in float impactForceMagnitude)
			{
				_impactForceMagnitudeCache.Add(impactForceMagnitude);

				if (_impactForceMagnitudeCache.Count > MAX_DATA_POINTS)
				{
					_impactForceMagnitudeCache.RemoveAt(0);
				}

				Repaint();
			}

			EggImpactDetector _eggImpactDetector;
			Color _graphBackgroundColor = new(0.15f, 0.15f, 0.15f, 1.0f);
			GUIStyle _thresholdLabelStyle = new();
			Color _thresholdColor = Color.yellow;
			GUIStyle _dataPointLabelStyle = new();
			Color _dataPointColor = Color.green;
			const int MAX_DATA_POINTS = 20;
			List<float> _impactForceMagnitudeCache = new();
		}
	}
}

#endif
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC
{
	public partial class LifespanHandler : MonoBehaviour
	{
		/// <summary>
		/// <see cref="LifespanHandler"/> 컴포넌트 커스텀 인스펙터 (단순 게이지 표시용)
		/// </summary>
		[CustomEditor(typeof(LifespanHandler))]
		private class LifespanHandlerEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				var lifespan = target as LifespanHandler;

				if (lifespan != null)
				{
					EditorGUILayout.Space(10);

					EditorGUILayout.LabelField("수명 진행 상황");

					var rect = GUILayoutUtility.GetRect(18, 18);

					EditorGUI.ProgressBar
					(
						rect,
						lifespan.LifespanRatio,
						$"{lifespan._currentLifespan:F2}s/{lifespan._maxLifespan:F2}s ({lifespan.LifespanRatio:P2})"
					);

					var thresholdXPos = rect.x + rect.width * (lifespan._mutationThreshold / lifespan._maxLifespan);

					EditorGUI.DrawRect(new Rect(thresholdXPos, rect.y, 1, rect.height), _mutationThresholdColor);

					if (Application.isPlaying)
					{
						Repaint();
					}
				}
			}

			readonly Color _mutationThresholdColor = new(1.0f, 0.0f, 0.0f, 0.7f);
		} // inner class
	} // class
} // namespace

#endif
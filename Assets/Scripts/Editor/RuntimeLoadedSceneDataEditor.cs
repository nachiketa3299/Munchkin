#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MC
{
	[CustomEditor(typeof(RuntimeLoadedSceneData))]
	public class RuntimeLoadedSceneDataEditor : Editor
	{
		#region UnityCallbacks

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var runtimeLoadedSceneData = target as RuntimeLoadedSceneData;

			var dict = runtimeLoadedSceneData.LoadedScenesByGameObject;

			if (dict.Count == 0)
			{
				return;
			}

			EditorGUILayout.LabelField("Loaded Scenes by GameObject");
			var ret = string.Empty;

			foreach(var (objName, sceneNameSets) in dict)
			{
				ret += $"<b>{objName}</b>:\n\t{string.Join(',', sceneNameSets)}\n";
			}

			var style = new GUIStyle(EditorStyles.textArea)
			{
				richText = true
			};

			EditorGUILayout.BeginScrollView(new(), GUILayout.Height(300.0f));
			EditorGUILayout.SelectableLabel(ret, style, GUILayout.ExpandHeight(true));
			EditorGUILayout.EndScrollView();
		}

		#endregion // UnityCallbacks
	}
}

#endif
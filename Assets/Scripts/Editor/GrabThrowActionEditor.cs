#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC.Editors
{
	[CustomEditor(typeof(GrabThrowAction))]
	internal sealed class GrabThrowActionEditor : Editor
	{
		#region UnityCallbacks

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			base.OnInspectorGUI();

			var grabThrowAction = target as GrabThrowAction;

			if (EditorApplication.isPlaying)
			{
				if (GUILayout.Button("Call EndAction"))
				{
					grabThrowAction.EndAction();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		#endregion // UnityCallbacks
	}
}

#endif
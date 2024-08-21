#if UNITY_EDITOR

using UnityEditor;

namespace MC.Editors
{
	[CustomEditor(typeof(SoulMovementHandler))]
	internal sealed class SoulMovementHandlerEditor : Editor
	{

		#region UnityCallbacks
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			base.OnInspectorGUI();
			var soulMovementHandler = target as SoulMovementHandler;

			serializedObject.ApplyModifiedProperties();
		}

		#endregion // UnityCallbacks

	}
}

#endif
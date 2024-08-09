#if UNITY_EDITOR

using UnityEditor;

namespace MC.Editors
{

[CustomEditor(typeof(MutationHandler))]
internal sealed class MutationHandlerEditor : Editor
{
		#region UnityCallbacks

		public override bool RequiresConstantRepaint() => true;

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			base.OnInspectorGUI();

			var characterType = (ECharacterType) serializedObject.FindProperty("_currentCharacterType").enumValueIndex;

			EditorGUI.BeginDisabledGroup(true);

				EditorGUILayout.EnumPopup("Character Type", characterType);

				var visualInstance = serializedObject.FindProperty("_currentVisualInstance").objectReferenceValue;

				EditorGUILayout.ObjectField
				(
					label: "Visual Instance",
					obj: visualInstance,
					objType: typeof(UnityEngine.Object),
					allowSceneObjects: true
				);

			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}

		#endregion // UnityCallbacks
	}

}

#endif
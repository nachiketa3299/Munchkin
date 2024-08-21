#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC.Editors
{

[CustomEditor(typeof(EggLifecycleHandler))]
internal sealed class EggLifecycleHandlerEditor : Editor
{

#region UnityCallbacks

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

		EditorGUI.BeginDisabledGroup(true);

		var owner = (EEggOwner) serializedObject.FindProperty("_owner").enumValueIndex;

		EditorGUILayout.EnumPopup("Owner", owner);

		EditorGUI.EndDisabledGroup();

		if (EditorApplication.isPlaying)
		{

			if (GUILayout.Button("End lifecycle"))
			{
				var eggLifecycleHandler = target as EggLifecycleHandler;
				if (eggLifecycleHandler.isActiveAndEnabled)
				{
					if (eggLifecycleHandler.TryGetComponent<GrabThrowTarget>(out var grabTarget) && grabTarget.IsGrabbed)
					{
						eggLifecycleHandler.EndLifecycle(true, grabTarget.Grabber);
					}
					else
					{
						eggLifecycleHandler.EndLifecycle();
					}
				}
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}

#endregion // UnityCallbacks

}

#endif
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC.Editors
{

[CustomEditor(typeof(EggPhysicalData))]
internal sealed class EggPhysicalDataEditor : Editor
{
	SerializedProperty _eggPrefabProperty;
	SerializedProperty _combinedPhysicalBoundsProperty;

	#region UnityCallbacks

	void OnEnable()
	{
		_eggPrefabProperty = serializedObject.FindProperty("_eggPrefab");
		_combinedPhysicalBoundsProperty = serializedObject.FindProperty("_combinedPhysicalBounds");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update(); // Takes the current state of real object, and updates the SerializedObject so that it has the same state

		if (_eggPrefabProperty == null)
		{
			Debug.LogError($"{_eggPrefabProperty}를 찾을 수 없습니다.");
			return;
		}

		if (_combinedPhysicalBoundsProperty == null)
		{
			Debug.LogError($"{_combinedPhysicalBoundsProperty}를 찾을 수 없습니다.");
			return;
		}

		base.OnInspectorGUI();

		if (GUILayout.Button("Cache egg physical bound"))
		{
			var combinedBounds = CalculatedCombinedBounds();
			_combinedPhysicalBoundsProperty.boundsValue = combinedBounds;
		}

		// Show calculated Bounds.extent info in inspector

		EditorGUILayout.Space();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUI.BeginDisabledGroup(true);

		if (_combinedPhysicalBoundsProperty != null)
		{
			EditorGUILayout.Vector3Field
			(
				label: "Combined Bounds Extents",
				value: _combinedPhysicalBoundsProperty.boundsValue.extents
			);
		}

		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
	}

	#endregion // UnityCallbacks

	Bounds CalculatedCombinedBounds()
	{
			var eggPrefab = _eggPrefabProperty.objectReferenceValue as EggLifecycleHandler;

			if (eggPrefab == null)
			{
				Debug.LogError("Egg Prefab이 할당되지 않았습니다.");
				return new();
			}

			// Calculate bounds with renderer

			var renderers = eggPrefab.GetComponentsInChildren<Renderer>();
			var combinedBounds = renderers[0].localBounds; // Collider.bounds only exists in instantiated object (prefab is not)

			for (var i = 1; i < renderers.Length; ++i)
			{
				combinedBounds.Encapsulate(renderers[i].localBounds);
			}

			return combinedBounds;
	}

}

} // namespace

#endif
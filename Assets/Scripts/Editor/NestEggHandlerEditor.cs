#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC
{

public partial class NestEggHandler : MonoBehaviour
{
	// For drawing nest egg spawn position with egg's physical bounds (what if it collides initially?)
	[SerializeField] EggPhysicalData _eggPhysicalData;

	[DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
	static void DrawSpawnPosition(NestEggHandler target, GizmoType gizmoType)
	{
		Gizmos.color = Color.magenta;

		if (target._eggPhysicalData)
		{
			var bounds = target._eggPhysicalData.CombinedPhysicalBounds;
			var spawnPosition = target.SpawnPosition + bounds.center;
			Gizmos.DrawWireCube(spawnPosition, bounds.size);
		}
		else
		{
			var spawnPosition = target.SpawnPosition;
			Gizmos.DrawWireCube(spawnPosition, Vector3.one);
		}
	}

	[DrawGizmo(GizmoType.Selected)]
	static void DrawNestEggName(NestEggHandler target, GizmoType gizmoType)
	{

		if (target._eggPool == null)
		{
			return;
		}

		Gizmos.color = Color.magenta;

		foreach(var nestEgg in target._eggPool.NestEggs)
		{
			if (nestEgg == null)
			{
				continue;
			}

			Gizmos.DrawLine
			(
				target.SpawnPosition,
				nestEgg.transform.position
			);
		}
	}
}

}

namespace MC.Editors
{

[CustomEditor(typeof(NestEggHandler))]
internal sealed class NestEggHandlerEditor : Editor
{

#region UnityCallbacks

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();

// Spawn nest egg button

		GUI.enabled = EditorApplication.isPlaying;

		EditorGUILayout.Space();

		if (GUILayout.Button("Spawn egg in nest"))
		{
			var nest = target as NestEggHandler;
			nest.SendMessage("SpawnNestEggs", 1);
		}

		GUI.enabled = true;

		serializedObject.ApplyModifiedProperties();
	}

#endregion // UnityCallbacks

}

}

#endif
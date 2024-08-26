#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC
{
public partial class NestEggHandler : MonoBehaviour
{
	// For drawing nest egg spawn position with egg's physical bounds (what if it collides initially?)
	[SerializeField] EggPhysicalData _eggPhysicalData;

	[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
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

		if (EggPool.Instance == null)
		{
			return;
		}

		Gizmos.color = Color.magenta;

		foreach(var nestEgg in EggPool.Instance.NestEggs)
		{
			if (nestEgg == null)
			{
				continue;
			}

			Gizmos.DrawLine(target.SpawnPosition, nestEgg.transform.position);
		}
	}
}

}

#endif
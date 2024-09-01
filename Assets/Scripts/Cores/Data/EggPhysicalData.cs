using UnityEngine;

namespace MC
{
	[CreateAssetMenu(fileName = "EggPhysicalData", menuName = "MC/Scriptable Objects/EggPhysicalData")]
	public class EggPhysicalData : ScriptableObject
	{
		public Bounds CombinedPhysicalBounds
		{
			get => _combinedPhysicalBounds;
			set => _combinedPhysicalBounds = value;
		}
		[HideInInspector] [SerializeField] Bounds _combinedPhysicalBounds;
		[SerializeField] EggLifecycleHandler _eggPrefab;
	}
}
using UnityEngine;

namespace MC
{
	[CreateAssetMenu(fileName = "EggPhysicalData", menuName = "MC/Scriptable Objects/EggPhysicalData")]
	public class EggPhysicalData : ScriptableObject
	{

		[SerializeField] EggLifecycleHandler _eggPrefab;
		public Bounds CombinedPhysicalBounds
		{
			get => _combinedPhysicalBounds;
			set => _combinedPhysicalBounds = value;
		}
		[HideInInspector] [SerializeField] Bounds _combinedPhysicalBounds;
	}
}
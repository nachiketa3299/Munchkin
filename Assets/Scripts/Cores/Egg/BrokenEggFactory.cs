using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public class BrokenEggFactory : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{

// Check Data

#if UNITY_EDITOR
		if (!_runtimePooledBrokenEggData)
		{
			Debug.LogWarning("RuntimePooledBrokenEggData를 찾을 수 없습니다.");
		}
#endif

	}

#endregion // UnityCallbacks

	public void SpawnInitializedBrokenEggFromPool(in FEggPhysicalState lastEggPhysicalState)
	{
		var brokenEgg = _runtimePooledBrokenEggData.Pool.Get();
		brokenEgg.InitializeLifecycle(lastEggPhysicalState);
	}

	public void ReturnBrokenEggToPool(BrokenEggLifecycleHandler brokenEgg)
	{
		// Do additional deinitialize in here

		_runtimePooledBrokenEggData.Pool.Release(brokenEgg);
	}

	[SerializeField] RuntimePooledBrokenEggData _runtimePooledBrokenEggData;
}

}

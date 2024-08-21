using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public class BrokenEggFactory : MonoBehaviour
{

#region UnityCallbacks

#if UNITY_EDITOR
	void Awake()
	{

		// Check Data

		if (!_runtimePooledBrokenEggData)
		{
			Debug.LogWarning("RuntimePooledBrokenEggData를 찾을 수 없습니다.");
		}
	}
#endif

#endregion // UnityCallbacks

	public void TakeFromPool(EggLastState lastEggPhysicalState)
	{
		var brokenEgg = _runtimePooledBrokenEggData.Pool.Get();
		brokenEgg.Initialize(lastEggPhysicalState);
	}

	public void ReturnToPool(BrokenEggLifecycleHandler brokenEgg)
	{
		// Do additional deinitialize in here

		_runtimePooledBrokenEggData.Pool.Release(brokenEgg);
	}

	[SerializeField] RuntimePooledBrokenEggData _runtimePooledBrokenEggData;
}

}

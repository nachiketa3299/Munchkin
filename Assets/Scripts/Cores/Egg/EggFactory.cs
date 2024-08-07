using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public class EggFactory : MonoBehaviour
{
	#region UnityCallbacks

	void Awake()
	{
		// Check data

#if UNITY_EDITOR
		if (!_runtimePooledEggData)
		{
			Debug.LogWarning("RuntimePooledEggData 가 설정되지 않았습니다.");
		}
#endif
	}

	#endregion // UnityCallbacks

	public GameObject TakeInitializedEggFromPool(EEggOwner owner)
	{
		var egg = _runtimePooledEggData.Pool.Get();
		egg.Initialize(owner);

		return egg.gameObject;
	}

	public void ReturnEggToPool(EggLifecycleHandler egg)
	{
		// egg.Deinitialize
		_runtimePooledEggData.Pool.Release(egg);

	}

	[SerializeField] RuntimePooledEggData _runtimePooledEggData;
}

}
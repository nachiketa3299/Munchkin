using UnityEngine;
using UnityEngine.Pool;

namespace MC
{

/// <summary>
/// *깨진 알* 인스턴스들을 관리하는 풀
/// </summary>
[CreateAssetMenu(fileName = "RuntimePooledBrokenEggData", menuName = "MC/Scriptable Objects/Runtime Pooled Broken Egg Data")]
public class RuntimePooledBrokenEggData : ScriptableObject
{
	// TODO 일정 갯수를 넘어서는 경우 풀로 반납되도록. (월드에 깨진 알이 최대 N개만 존재할 수 있도록)
	public ObjectPool<BrokenEggLifecycleHandler> Pool
	{
		get
		{
			return _brokenEggPool ??= new
			(
				createFunc: CreateInstance,
				actionOnGet: TakeFromPool,
				actionOnRelease: ReturnToPool,
				actionOnDestroy: DestroyInstance,
				collectionCheck: true,
				defaultCapacity: _defaultCapacity
			);
		}
	}

	BrokenEggLifecycleHandler CreateInstance()
	{

#if UNITY_EDITOR
	if (!_brokenEggPrefab)
	{
		Debug.Log("BrokenEggPrefab을 찾을 수 없습니다.");
	}
#endif

		var instance = Instantiate(_brokenEggPrefab);
		instance.gameObject.name = $"BrokenEgg({instance.GetInstanceID()})";
		return instance;
	}

	void TakeFromPool(BrokenEggLifecycleHandler brokenEgg)
	{
		brokenEgg.gameObject.SetActive(true);
	}

	void ReturnToPool(BrokenEggLifecycleHandler brokenEgg)
	{
		brokenEgg.gameObject.SetActive(false);
	}

	void DestroyInstance(BrokenEggLifecycleHandler brokenEgg)
	{
		Destroy(brokenEgg.gameObject);
	}

	ObjectPool<BrokenEggLifecycleHandler> _brokenEggPool;
	[SerializeField] int _defaultCapacity = 5;
	[SerializeField] BrokenEggLifecycleHandler _brokenEggPrefab;
}

}
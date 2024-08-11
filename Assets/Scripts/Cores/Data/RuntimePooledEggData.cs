using UnityEngine;
using UnityEngine.Pool;

namespace MC
{

/// <summary>
/// 게임 전체에서 생성되는 알 인스턴스들의 풀을 관리
/// </summary>
[CreateAssetMenu(fileName = "RuntimePooledEggData", menuName = "MC/Scriptable Objects/Runtime Pooled Egg Data")]
public class RuntimePooledEggData : ScriptableObject
{
	public ObjectPool<EggLifecycleHandler> Pool
	{
		get
		{
			return _eggPool ??=  new
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

	/// <summary>
	/// CharacterEggPool에 새 인스턴스를 생성할 때 실행되는 로직
	/// </summary>
	EggLifecycleHandler CreateInstance()
	{

	#if UNITY_EDITOR
		if (!_eggPrefab)
		{
			Debug.LogWarning("EggPrefab을 찾을 수 없습니다.");
		}
	#endif

		var instance = Instantiate(_eggPrefab);
		return instance;
	}

	/// <summary>
	/// Pool에 저장된 인스턴스를 가져올 때 실행되는 로직
	/// </summary>
	void TakeFromPool(EggLifecycleHandler egg)
	{
		egg.gameObject.SetActive(true);
		egg.GetComponent<Rigidbody>().isKinematic = false; // TODO 다른 더 적절한 옮겨야 함
	}

	/// <summary>
	/// Pool에 모두 사용된 인스턴스를 반납할 때 실행되는 로직
	/// </summary>
	void ReturnToPool(EggLifecycleHandler egg)
	{
		if (egg.transform.parent != null)
		{
			egg.transform.SetParent(null);
		}

		egg.gameObject.SetActive(false);
		egg.GetComponent<Rigidbody>().isKinematic = true; // TODO 다른 더 적절한 곳으로 옮겨야 함
	}

	/// <summary>
	/// Pool 에서 객체를 파괴할 때 실행되는 로직
	/// </summary>
	void DestroyInstance(EggLifecycleHandler egg)
	{
		Destroy(egg.gameObject);
	}

	ObjectPool<EggLifecycleHandler> _eggPool;
	[SerializeField] int _defaultCapacity = 5;
	[SerializeField] EggLifecycleHandler _eggPrefab;
}

}
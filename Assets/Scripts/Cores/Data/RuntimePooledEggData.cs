using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Pool;

namespace MC
{

/// <summary>
/// 게임 전체에서 생성되는 알 인스턴스들의 풀을 관리
/// </summary>
/// <remarks>
/// 이 객체에서는 오직 풀에 관련된 연산만 처리하고, 초기화에 관련된 내용은 <see cref="EggFactory"/>에서 담당한다.
/// </remarks>
[CreateAssetMenu(fileName = "RuntimePooledEggData", menuName = "MC/Scriptable Objects/Runtime Pooled Egg Data")]
public class RuntimePooledEggData : ScriptableObject
{
	public Action<EggLifecycleHandler> NestEggEnabled;
	public Action NestEggDisabled;

	public ObjectPool<EggLifecycleHandler> Pool
	{
		get
		{
			return _eggPool ??=  new
			(
				createFunc: CreateEggInstance,
				actionOnGet: TakeEggFromPool,
				actionOnRelease: ReturnEggToPool,
				actionOnDestroy: DestroyEggInstance,
				collectionCheck: true,
				defaultCapacity: _defaultPoolCapacity,
				maxSize: _maxPoolSize
			);
		}
	}

#region ObjectPoolCallbacks

	EggLifecycleHandler CreateEggInstance()
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

	void TakeEggFromPool(EggLifecycleHandler egg)
	{
		egg.gameObject.SetActive(true);
	}

	void ReturnEggToPool(EggLifecycleHandler egg)
	{
		egg.gameObject.SetActive(false);
	}

	void DestroyEggInstance(EggLifecycleHandler egg)
	{
		Destroy(egg.gameObject);
	}

#endregion // ObjectPoolCallbacks

	public EggLifecycleHandler Get(EEggOwner owner)
	{
		var egg = Pool.Get();

		// Initialize
		egg.Initialize(owner);

		AddToContainer(egg);
		return egg;
	}

	public void Release(EggLifecycleHandler egg)
	{
		RemoveFromContainer(egg);
		// Deinitialize
		egg.Deinitialize();
		Pool.Release(egg);

	}

	void AddToContainer(EggLifecycleHandler egg)
	{
		switch(egg.Owner)
		{
			case EEggOwner.Character:
				_characterEggs.Add(egg);
				break;
			case EEggOwner.Nest:
				_nestEggs.Add(egg);
				break;
		}
	}

	void RemoveFromContainer(EggLifecycleHandler egg)
	{
		switch(egg.Owner)
		{
			case EEggOwner.Character:
				_characterEggs.Remove(egg);
				break;
			case EEggOwner.Nest:
				_nestEggs.Remove(egg);
				NestEggDisabled?.Invoke();
				break;
		}
	}

	// Don't fucking modify this outside
	public ReadOnlyCollection<EggLifecycleHandler> NestEggs => _nestEggs.AsReadOnly();

	ObjectPool<EggLifecycleHandler> _eggPool;
	[SerializeField] int _defaultPoolCapacity = 5;
	[SerializeField] int _maxPoolSize = 10;
	[SerializeField] EggLifecycleHandler _eggPrefab;
	[SerializeField][HideInInspector] List<EggLifecycleHandler> _characterEggs = new();
	// 여기서 리스트를 택한 이유는 직렬화 때문
	[SerializeField][HideInInspector] List<EggLifecycleHandler> _nestEggs = new();
}

}
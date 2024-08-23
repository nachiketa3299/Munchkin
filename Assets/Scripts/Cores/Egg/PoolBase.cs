using System;

using UnityEngine;
using UnityEngine.Pool;

namespace MC
{

public abstract class PoolBase<T> : MonoBehaviour where T : MonoBehaviour
{

	public event Action<T> InstanceEnabled;
	public event Action InstanceDisabled;

	protected void RaiseInstanceDisabled() => InstanceDisabled?.Invoke();
	protected void RaiseInstanceEnabled(T instance) => InstanceEnabled?.Invoke(instance);

#region UnityCallbacks

	protected virtual void Awake()
	{
#if UNITY_EDITOR
		if (!_prefab)
		{
			Debug.LogWarning("Pool에서 Prefab을 찾을 수 없습니다.");
		}
#endif

		InitializePool();
	}

#endregion // UnityCallbacks

	protected void InitializePool() => _pool = new (CreateInstance, TakeInstanceFromPool, ReturnInstanceToPool, DestroyInstance, true, _defaultPoolCapacity, _maxPoolSize);

#region ObjectPoolCallbacks

	protected virtual T CreateInstance() => Instantiate(_prefab);
	protected virtual void TakeInstanceFromPool(T instance) => instance.gameObject.SetActive(true);
	protected virtual void ReturnInstanceToPool(T instance) => instance.gameObject.SetActive(false);
	protected virtual void DestroyInstance(T instance) => Destroy(instance.gameObject);

#endregion // ObjectPoolCallbacks

	public ObjectPool<T> Pool => _pool;

	ObjectPool<T> _pool;
	[SerializeField] T _prefab;
	[SerializeField] int _defaultPoolCapacity = 5;
	[SerializeField] int _maxPoolSize = 10;

}

}
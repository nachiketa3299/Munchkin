using UnityEngine;
using UnityEngine.Pool;

namespace MC
{
	[CreateAssetMenu(fileName = "RuntimePooledEggData", menuName = "MC/Scriptable Objects/Runtime Pooled Egg Data")]
	public class RuntimePooledEggData : ScriptableObject
	{
		public IObjectPool<EggLifeCycleHandler> Pool
		{
			get
			{
				if (_pool == null)
				{
					_pool = new
					(
						createFunc: CreateInstance,
						actionOnGet: TakeFromPool,
						actionOnRelease: ReturnToPool,
						actionOnDestroy: DestroyInstance,
						collectionCheck: true,
						defaultCapacity: _poolCapacity
					);
				}
				return _pool;
			}
		}
		public bool IsPoolInitialized => _pool != null;
		public int CountInactive => _pool.CountInactive;
		public int CountActive => _pool.CountActive;
		public int CountAll => _pool.CountAll;

		EggLifeCycleHandler CreateInstance()
		{
			return Instantiate(_eggPrefab);
		}

		void TakeFromPool(EggLifeCycleHandler egg)
		{
			egg.gameObject.SetActive(true);
		}

		void ReturnToPool(EggLifeCycleHandler egg)
		{
			egg.gameObject.SetActive(false);
		}

		void DestroyInstance(EggLifeCycleHandler egg)
		{
			Destroy(egg.gameObject);
		}


		[SerializeField] EggLifeCycleHandler _eggPrefab;
		ObjectPool<EggLifeCycleHandler> _pool;
		[SerializeField] int _poolCapacity = 5;
	}
}
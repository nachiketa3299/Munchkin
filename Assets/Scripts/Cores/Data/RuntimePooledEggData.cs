using UnityEngine;
using UnityEngine.Pool;

namespace MC
{
	/// <summary>
	/// 게임 전체에서 생성되는 알 인스턴스들의 풀을 관리
	/// </summary>
	[CreateAssetMenu(fileName = "RuntimePooledEggData", menuName = "MC/Scriptable Objects/Runtime Pooled Egg Data")]
	public partial class RuntimePooledEggData : ScriptableObject
	{
		EggLifecycleHandler CreateInstance()
		{
			var instance = Instantiate(_eggPrefab);
			instance.Initialize();

			return instance;
		}
		void TakeFromPool(EggLifecycleHandler egg)
		{
			egg.gameObject.SetActive(true);
			egg.Initialize();
		}
		void ReturnToPool(EggLifecycleHandler egg)
		{
			egg.gameObject.SetActive(false);
		}
		void DestroyInstance(EggLifecycleHandler egg)
		{
			Destroy(egg.gameObject);
		}

		public IObjectPool<EggLifecycleHandler> Pool
		{
			get
			{
				_pool ??= new
				(
					createFunc: CreateInstance,
					actionOnGet: TakeFromPool,
					actionOnRelease: ReturnToPool,
					actionOnDestroy: DestroyInstance,
					collectionCheck: true,
					defaultCapacity: _defaultPoolCapacity
				);

				return _pool;
			}
		}

		ObjectPool<EggLifecycleHandler> _pool;
		[SerializeField] int _defaultPoolCapacity = 5;
		[SerializeField] EggLifecycleHandler _eggPrefab;

		/// <remarks>
		/// 에디터에서 수동으로 계산된 것(버튼을 눌러라)이 캐싱된다.
		/// </remarks>
		public Bounds EggCombinedPhysicalBounds
		{
			get => _eggCombinedPhysicalBounds;
			set => _eggCombinedPhysicalBounds = value;
		}
		Bounds _eggCombinedPhysicalBounds;
	}
}
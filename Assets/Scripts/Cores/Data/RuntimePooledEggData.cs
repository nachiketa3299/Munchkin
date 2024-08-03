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
		public ObjectPool<EggLifecycleHandler> CharacterEggPool
		{
			get
			{
				return _characterEggPool ??=  new
				(
					createFunc: CreateCharacterEggInstance,
					actionOnGet: TakeFromPool,
					actionOnRelease: ReturnToPool,
					actionOnDestroy: DestroyInstance,
					collectionCheck: true,
					defaultCapacity: _characterEggPoolDefaultCapacity
				);
			}
		}

		public ObjectPool<EggLifecycleHandler> NestEggPool
		{
			get
			{
				return _nestEggPool ??= new
				(
					createFunc: CreateNestEggInstance,
					actionOnGet: TakeFromPool,
					actionOnRelease: ReturnToPool,
					actionOnDestroy: DestroyInstance,
					collectionCheck: true,
					defaultCapacity: _nestEggPoolDefaultCapacity
				);
			}
		}

		/// <summary>
		/// CharacterEggPool에 새 인스턴스를 생성할 때 실행되는 로직
		/// </summary>
		EggLifecycleHandler CreateCharacterEggInstance()
		{
			var instance = Instantiate(_characterEggPrefab);

			return instance;
		}

		/// <summary>
		/// NestEggPool에 새 인스턴스를 생성할 때 실행되는 로직
		/// </summary>
		/// <returns></returns>
		EggLifecycleHandler CreateNestEggInstance()
		{
			var instance = Instantiate(_nestEggPrefab);

			return instance;
		}

		/// <summary>
		/// Pool에 저장된 인스턴스를 가져올 때 실행되는 로직
		/// </summary>
		void TakeFromPool(EggLifecycleHandler egg)
		{
			egg.gameObject.SetActive(true);
			egg.Initialize();
		}

		/// <summary>
		/// Pool에 모두 사용된 인스턴스를 반납할 때 실행되는 로직
		/// </summary>
		void ReturnToPool(EggLifecycleHandler egg)
		{
			egg.gameObject.SetActive(false);
		}

		/// <summary>
		/// Pool 에서 객체를 파괴할 때 실행되는 로직
		/// </summary>
		void DestroyInstance(EggLifecycleHandler egg)
		{
			Destroy(egg.gameObject);
		}

		ObjectPool<EggLifecycleHandler> _characterEggPool;
		ObjectPool<EggLifecycleHandler> _nestEggPool;
		[SerializeField] int _characterEggPoolDefaultCapacity = 5;
		[SerializeField] int _nestEggPoolDefaultCapacity = 5;
		[SerializeField] EggLifecycleHandler _characterEggPrefab;
		[SerializeField] EggLifecycleHandler _nestEggPrefab;
	}
}
using UnityEngine;
using UnityEngine.SceneManagement;

// <기본 전제>
// Egg 라는 것은, Pool을 거치지 않고는 생성이 불가능함
// NestEgg는 반드시 NestEggHandler 에게서만 생성되고, 초기 생성시 영역 내에 배치됨
// 생성된 NestEgg 들의 목록은 Pool에 질의하면 됨. (선형으로, 생성 순서대로 리스트에 배치되어 있음)
// 당연히 들어올 때

namespace MC
{

/// <summary>
/// Nest 내부의 알의 갯수가 일정하게 유지되도록 관리한다.
/// </summary>
[DisallowMultipleComponent]
public partial class NestEggHandler : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{

#if UNITY_EDITOR
		if (!_eggPool)
		{
			Debug.LogWarning("NestEggHandler에서 RuntimePooledEggData를 찾을 수 없습니다.");
		}
#endif

		// Start는 Scene이 Active 되기 이전에 실행되기 때문에, Nest Egg가 PersistentScene 이전에 활성화 되어 있던 씬에 생성됨을 주의.
		SceneManager.activeSceneChanged += OnPersistentSceneActivated;
		_eggPool.NestEggDisabled += CheckAndSpawn;
	}

	void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnPersistentSceneActivated;
		_eggPool.NestEggDisabled -= CheckAndSpawn;
	}

#endregion // UnityCallbacks

	void OnPersistentSceneActivated(Scene _1, Scene _2)
	{
		CheckAndSpawn();
	}

	void CheckAndSpawn()
	{
		var initialCount = _eggPool.NestEggs.Count;
		var delta = _nestEggCountToMainTain - initialCount;

		for (var i = 0; i < delta; ++i)
		{
			var instance = _eggPool.Get(EEggOwner.Nest);
			instance.transform.SetPositionAndRotation(SpawnPosition, Quaternion.identity);
		}
	}

	public Vector3 SpawnPosition => _spawnPosition + transform.position;

	[SerializeField] int _nestEggCountToMainTain = 2;
	[SerializeField] Vector3 _spawnPosition = new();
	[SerializeField] RuntimePooledEggData _eggPool;

}

}
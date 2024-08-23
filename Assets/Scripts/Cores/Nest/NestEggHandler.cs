using UnityEngine;
using UnityEngine.SceneManagement;

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
		// Start는 Scene이 Active 되기 이전에 실행되기 때문에, Nest Egg가 PersistentScene 이전에 활성화 되어 있던 씬에 생성됨을 주의.
		SceneManager.activeSceneChanged += OnPersistentSceneActivated;
	}

	void Start()
	{
		EggPool.Instance.InstanceDisabled += CheckAndSpawn;

		// CheckAndSpawn(); TODO 에디터에서 Persistent 만 올려두고 실행하는 경우 오류
	}

	void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnPersistentSceneActivated;
		EggPool.Instance.InstanceDisabled -= CheckAndSpawn;
	}

#endregion // UnityCallbacks

	void OnPersistentSceneActivated(Scene _1, Scene _2)
	{
		CheckAndSpawn();
	}

	void CheckAndSpawn()
	{
		var initialCount = EggPool.Instance.NestEggs.Count;
		var delta = _nestEggCountToMainTain - initialCount;

		for (var i = 0; i < delta; ++i)
		{
			var instance = EggPool.Instance.GetEggInstance(EEggOwner.Nest);
			instance.transform.SetPositionAndRotation(SpawnPosition, Quaternion.identity);
		}
	}

	public Vector3 SpawnPosition => _spawnPosition + transform.position;

	[SerializeField] int _nestEggCountToMainTain = 2;
	[SerializeField] Vector3 _spawnPosition = new();

}

}
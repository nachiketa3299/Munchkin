using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
[RequireComponent(typeof(EggFactory))]
[RequireComponent(typeof(NestTrigger))]
public class NestEggHandler : MonoBehaviour
{
	public delegate void NestEggEventHandler();
	public NestEggEventHandler NestIsEmpty;

	#region UnityCallbacks

	void Awake()
	{
		// Cache components & data

		_eggFactory = GetComponent<EggFactory>();
		_nestTrigger = GetComponent<NestTrigger>();

		// Bind events

		NestIsEmpty += SpawnEggInNest;

		_nestTrigger.NestEggEnteredTrigger += OnNestEggEnteredTrigger;
		_nestTrigger.NestEggExitedTrigger += OnNestEggExitedTrigger;
	}

	void Start()
	{
		if (_nestEggs.Count != 0)
		{
			return;
		}

		// Nest is empty in start

		NestIsEmpty?.Invoke();
	}

	void OnDestroy()
	{
		NestIsEmpty -= SpawnEggInNest;

		_nestTrigger.NestEggEnteredTrigger -= OnNestEggEnteredTrigger;
		_nestTrigger.NestEggExitedTrigger -= OnNestEggExitedTrigger;
	}

	#endregion // UnityCallbacks

	void OnNestEggExitedTrigger(EggLifecycleHandler nestEgg)
	{
		_nestEggs.Remove(nestEgg);
		nestEgg.GetComponent<EggHealthManager>().ForceInflictLethalDamage();

		if (_nestEggs.Count == 0)
		{
			SpawnEggInNest();
		}
	}

	void OnNestEggEnteredTrigger(EggLifecycleHandler nestEgg)
	{
		Debug.Log("OnNestEggEnteredTrigger");
		_nestEggs.Add(nestEgg);

		if (_nestEggs.Count > _maxNestEggCount)
		{
			var toDestroy = _nestEggs.First();
			_eggFactory.ReturnEggToPool(toDestroy);
			_nestEggs.RemoveAt(0);
		}
	}

	public void SpawnEggInNest()
	{
		var spawnedInstance = _eggFactory.TakeInitializedEggFromPool(owner: EEggOwner.Nest);

		spawnedInstance.transform.SetPositionAndRotation(_spawnPositionObject.transform.position, Quaternion.identity);
	}

	[SerializeField][HideInInspector] GameObject _currentNestEgg;
	EggFactory _eggFactory;
	NestTrigger _nestTrigger;
	[SerializeField][HideInInspector] List<EggLifecycleHandler> _nestEggs = new();
	int _maxNestEggCount = 1;
	[SerializeField] GameObject _spawnPositionObject;
}

}
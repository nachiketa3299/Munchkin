using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public class EggPool : PoolBase<EggLifecycleHandler>
{

#region UnityCallbacks

	protected override void Awake()
	{
		base.Awake();

		Instance = this;
	}

#endregion // UnityCallbacks

	public static EggPool Instance { get; private set; }

	public EggLifecycleHandler GetEggInstance(EEggOwner owner)
	{
		var eggInstance = Pool.Get();

		// Initialize

		eggInstance.Initialize(owner);
		AddToContainer(eggInstance);
		return eggInstance;
	}

	public void ReleaseEggInstance(EggLifecycleHandler eggInstance)
	{
		RemoveFromContainer(eggInstance);
		// Deinitialize
		eggInstance.Deinitialize();
		Pool.Release(eggInstance);
	}

	void AddToContainer(EggLifecycleHandler eggInstance)
	{
		switch(eggInstance.Owner)
		{
			case EEggOwner.Character:
				_characterEggs.Add(eggInstance);
				break;
			case EEggOwner.Nest:
				_nestEggs.Add(eggInstance);
				break;
		}
	}

	void RemoveFromContainer(EggLifecycleHandler eggInstance)
	{
		switch(eggInstance.Owner)
		{
			case EEggOwner.Character:
				_characterEggs.Remove(eggInstance);
				break;
			case EEggOwner.Nest:
				_nestEggs.Remove(eggInstance);
				RaiseInstanceDisabled();
				break;
		}
	}

	public ReadOnlyCollection<EggLifecycleHandler> NestEggs => _nestEggs.AsReadOnly();
	public ReadOnlyCollection<EggLifecycleHandler> CharacterEggs => _characterEggs.AsReadOnly();

	// 여기서 리스트를 택한 이유는 직렬화 때문
	[SerializeField][HideInInspector] List<EggLifecycleHandler> _characterEggs = new();
	[SerializeField][HideInInspector] List<EggLifecycleHandler> _nestEggs = new();
}


}
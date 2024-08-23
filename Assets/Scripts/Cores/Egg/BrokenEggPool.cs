using System.Collections.Generic;

using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public class BrokenEggPool : PoolBase<BrokenEggLifecycleHandler>
{

#region UnityCallbacks

	protected override void Awake()
	{
		base.Awake();

		Instance = this;
	}

#endregion

	public static BrokenEggPool Instance { get; private set; }

	public BrokenEggLifecycleHandler GetBrokenEggInstance(EggLastState eggLastState)
	{
		var brokenEggInstance = Pool.Get();
		brokenEggInstance.Initialize(eggLastState);

		_brokenEggs.Add(brokenEggInstance);

		return brokenEggInstance;
	}

	public void ReleaseBrokenEggInstance(BrokenEggLifecycleHandler brokenEggInstance)
	{
		_brokenEggs.Remove(brokenEggInstance);
		brokenEggInstance.Deinitialize();
		Pool.Release(brokenEggInstance);
	}

	[SerializeField][HideInInspector] List<BrokenEggLifecycleHandler> _brokenEggs = new();
}

}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MC
{

/// <summary>
/// Nest의 트리거 볼륨에 들어가거나 나오는 <b>루트</b> 오브젝트들을 필터링하여 이벤트를 촉발
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class NestTrigger : MonoBehaviour
{
	public delegate void NestEggTriggerEventHandler(EggLifecycleHandler nestEgg);
	public NestEggTriggerEventHandler NestEggEnteredTrigger;
	public NestEggTriggerEventHandler NestEggExitedTrigger;

	#region UnityCallbacks

	void Awake()
	{
		// Cache components

		_collider = GetComponent<Collider>();
	}

	/// <summary>
	/// 트리거를 발동시키는 콜라이더의 루트 오브젝트가 Nest Egg 인지 아닌지에만 관심
	/// </summary>
	void OnTriggerEnter(Collider collider)
	{
		var rootObject = collider.transform.root.gameObject;

		Debug.Log($"{rootObject.name} entered nest trigger");

		// Egg 인가?
		var eggLifecycleHandler = rootObject.GetComponent<EggLifecycleHandler>();

		if (!eggLifecycleHandler)
		{
			return;
		}

		// Egg 라면, Nest Egg 인가?

		if (eggLifecycleHandler.Owner != EEggOwner.Nest)
		{
			return;
		}

		if(_uniqueStayingEggs.Contains(eggLifecycleHandler))
		{
			return;
		}

		NestEggEnteredTrigger?.Invoke(eggLifecycleHandler);
		_uniqueStayingEggs.Add(eggLifecycleHandler);
	}

	void FixedUpdate()
	{
		for (var i = 0; i < _uniqueStayingEggs.Count; ++i)
		{
			var eggLifecycleHandler = _uniqueStayingEggs[i];
			var physicalColliders = eggLifecycleHandler.GetComponentsInChildren<Collider>()
				.Where(collider => !collider.isTrigger)
				.ToArray();

			var combinedBound = physicalColliders[0].bounds;
			for (var j = 1; j < physicalColliders.Length; ++j)
			{
				combinedBound.Encapsulate(physicalColliders[j].bounds);
			}

			var maxPoint = combinedBound.max;
			maxPoint.z = 0.0f;
			var minPoint = combinedBound.min;
			minPoint.z = 0.0f;

			Debug.DrawLine(maxPoint, minPoint, Color.magenta);
			Debug.DrawLine(_collider.bounds.max, _collider.bounds.min, Color.green);

			var isContainingMaxPoint = _collider.bounds.Contains(maxPoint);
			var isContainingMinPoint = _collider.bounds.Contains(minPoint);

			if (isContainingMaxPoint && isContainingMinPoint)
			{
				Debug.Log("Inside");
				continue;
			}

			Debug.Log("Outside");

			NestEggExitedTrigger?.Invoke(eggLifecycleHandler);
			_uniqueStayingEggs.Remove(eggLifecycleHandler);
		}
	}

	#endregion // UnityCallbacks

	List<EggLifecycleHandler> _uniqueStayingEggs = new();
	Collider _collider;
}

}
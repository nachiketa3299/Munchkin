#if false
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public class EggBounds : MonoBehaviour
{

#region UnityCallbacks

	void Awake()
	{
		_colliders = GetComponentsInChildren<Collider>()
			.Where(collider => !collider.isTrigger)
			.ToList();
	}

#endregion // Unity Callbacks

	/// <remarks>
	/// 게임이 실행 중일 때에만 유효한 값을 반환한다.
	/// </remarks>
	public Bounds CalculateCombinedBounds()
	{
		if (_colliders == null)
		{
			return new();
		}

		if (_colliders.Count == 0)
		{
			return new();
		}

		if (_colliders.Count == 1)
		{
			return _colliders.First().bounds;
		}

		return _colliders
			.Select(collider => collider.bounds)
			.Aggregate((current, next) => {current.Encapsulate(next); return current;});
	}

	/// <remarks>
	/// 게임이 실행 중일 때에만 유효한 값을 반환한다.
	/// </remarks>
	public List<Bounds> GetAllBounds()
	{
		if (_colliders == null)
		{
			return new();
		}

		if (_colliders.Count == 0)
		{
			return new();
		}

		return _colliders
			.Select(collider => collider.bounds)
			.ToList();
	}
	List<Collider> _colliders;

	// TODO EggPhysicalData를 여기로 이관할 것
}

}

#endif
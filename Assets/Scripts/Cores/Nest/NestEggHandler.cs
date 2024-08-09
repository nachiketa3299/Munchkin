using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace MC
{

/// <summary>
/// Nest 내부의 알의 갯수가 일정하게 유지되도록 관리한다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(EggFactory))]
public class NestEggHandler : MonoBehaviour
{
	public delegate void NestEggQuantityEventHandler(int quantity);
	public event NestEggQuantityEventHandler NestEggSpawnNeeded;
	public event NestEggQuantityEventHandler NestEggDespawnNeeded;

#region UnityCallbacks

	void Awake()
	{
		// Cache components & data

		_eggFactory = GetComponent<EggFactory>();

		// Bind events

		NestEggSpawnNeeded += SpawnNestEggs;
		NestEggDespawnNeeded += DespawnNestEggs;

		SceneManager.activeSceneChanged += (_, _) => { CheckNestEggCounts();  };
	}

	void OnDestroy()
	{
		// Unbind events

		NestEggSpawnNeeded -= SpawnNestEggs;
		NestEggDespawnNeeded -= DespawnNestEggs;
	}

	void OnTriggerEnter(Collider collider)
	{
		// 루트오브젝트의 컴포넌트로 판별. (Egg에는 컴파운드 콜라이더가 존재)
		var incomingGameObject = collider.transform.root.gameObject;

		if (!incomingGameObject)
		{
			return;
		}

		var incomingEgg = incomingGameObject.GetComponent<EggLifecycleHandler>();

		if (!incomingEgg)
		{
			var grabSubject = incomingGameObject.GetComponent<GrabThrowAction>();

			if (!grabSubject)
			{
				return;
			}

			incomingEgg = grabSubject.GrabThrowTarget?.GetComponent<EggLifecycleHandler>();

			if (!incomingEgg)
			{
				return;
			}
		}

		if(incomingEgg.Owner != EEggOwner.Nest)
		{
			return;
		}

		incomingEgg.GetComponent<EggHealthManager>().ForceInflictLethalDamage();

		if (!_nestEggs.Contains(incomingEgg))
		{
			return;
		}

		_nestEggs.Remove(incomingEgg);

		CheckNestEggCounts();
	}

#endregion // UnityCallbacks

	void CheckNestEggCounts()
	{
		var delta = _maxNestEggCount - _nestEggs.Count;

		if (delta == 0)
		{
			return;
		}

		if (delta > 0)
		{
			NestEggSpawnNeeded?.Invoke(delta);
			return;
		}

		if (delta < 0)
		{
			NestEggDespawnNeeded?.Invoke(Mathf.Abs(delta));
			return;
		}
	}

#if UNITY_EDITOR
public
#endif
	 void SpawnNestEggs(int quantity)
	{
		for (var i = 0; i < quantity; ++i)
		{
			var spawnedNestEgg = _eggFactory.TakeInitializedEggFromPool(owner: EEggOwner.Nest);

			spawnedNestEgg.transform.SetPositionAndRotation
			(
				position: SpawnPosition,
				rotation: Quaternion.identity
			);

			_nestEggs.Add(spawnedNestEgg.GetComponent<EggLifecycleHandler>());
		}
	}

	public void DespawnNestEggs(int quantity)
	{
		for (var i = 0; i < quantity; ++i)
		{
			_nestEggs[i].GetComponent<EggHealthManager>().ForceInflictLethalDamage();
		}

		_nestEggs.RemoveRange(0, quantity);
	}

	EggFactory _eggFactory;
	Vector3 SpawnPosition => _spawnPosition + transform.position;
	[SerializeField][HideInInspector] List<EggLifecycleHandler> _nestEggs = new();
	[SerializeField] int _maxNestEggCount = 1;
	[SerializeField] Vector3 _spawnPosition = new();

#if UNITY_EDITOR
	[SerializeField] EggPhysicalData _eggPhysicalData;

	[DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
	static void DrawSpawnPosition(NestEggHandler target, GizmoType gizmoType)
	{
		Gizmos.color = Color.magenta;

		if (target._eggPhysicalData)
		{
			var bounds = target._eggPhysicalData.CombinedPhysicalBounds;
			var spawnPosition = target.SpawnPosition + bounds.center;
			Gizmos.DrawWireCube(spawnPosition, bounds.size);
		}
		else
		{
			var spawnPosition = target.SpawnPosition;
			Gizmos.DrawWireCube(spawnPosition, Vector3.one);
		}
	}

	[DrawGizmo(GizmoType.Selected)]
	static void DrawNestEggName(NestEggHandler target, GizmoType gizmoType)
	{
		// var style = new GUIStyle();
		// style.normal.textColor = Color.yellow;

		Gizmos.color = Color.magenta;

		foreach(var nestEgg in target._nestEggs)
		{
			Gizmos.DrawLine
			(
				target.SpawnPosition,
				nestEgg.transform.position
			);
		}

		// foreach(var nestEgg in target._nestEggs)
		// {
		// 	Handles.Label
		// 	(
		// 		position: nestEgg.transform.position,
		// 		text: $"{nestEgg.gameObject.name}",
		// 		style: style
		// 	);
		// }
	}

#endif

}

}
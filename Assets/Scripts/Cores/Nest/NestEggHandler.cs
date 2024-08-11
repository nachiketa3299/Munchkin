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

		// Start는 Scene이 Active 되기 이전에 실행되기 때문에, Nest Egg가 PersistentScene에 만들어지지 않으므로 주의
		SceneManager.activeSceneChanged += OnPersistentGameplaySceneLoadedAndActivated;
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
		var incomingEgg = incomingGameObject.GetComponent<EggLifecycleHandler>();

		// 알이고, 소유자가 둥지인가?
		if (incomingEgg && incomingEgg.Owner == EEggOwner.Nest)
		{
			DestroyTriggeredNestEgg(incomingEgg);
		}

		// Grabbed character
		else
		{
			var grabSubject = incomingGameObject.GetComponent<GrabThrowAction>();
			if (grabSubject)
			{
				var grabTarget = grabSubject.GrabThrowTarget;

				if (grabTarget)
				{
					var grabbedEgg = grabTarget.GetComponent<EggLifecycleHandler>();
					if (grabbedEgg.Owner == EEggOwner.Nest)
					{
						DestroyTriggeredGrabbedNestEgg(grabbedEgg, grabSubject);
					}
				}
			}
		}
	}

#endregion // UnityCallbacks
	void OnPersistentGameplaySceneLoadedAndActivated(Scene from, Scene to)
	{
		CheckNestEggCounts();
	}

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

	void DestroyTriggeredNestEgg(EggLifecycleHandler nestEgg)
	{
		nestEgg.LifecycleEndedByTriggerEvent?.Invoke (new FEggPhysicalState(nestEgg));

		if (_nestEggs.Contains(nestEgg))
		{
			_nestEggs.Remove(nestEgg);
			CheckNestEggCounts();
		}
	}

	void DestroyTriggeredGrabbedNestEgg(EggLifecycleHandler grabTarget, GrabThrowAction grabSubject)
	{
		grabTarget.LifecycleEndedByTriggerEvent?.Invoke(new FEggPhysicalState(grabTarget, grabSubject));

		if (_nestEggs.Contains(grabTarget))
		{
			_nestEggs.Remove(grabTarget);
			CheckNestEggCounts();
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

		Gizmos.color = Color.magenta;

		foreach(var nestEgg in target._nestEggs)
		{
			Gizmos.DrawLine
			(
				target.SpawnPosition,
				nestEgg.transform.position
			);
		}
	}

#endif

}

}
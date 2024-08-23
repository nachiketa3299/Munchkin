using UnityEngine;

namespace MC
{

/// <summary>
/// 둥지의 경계 콜라이더에 닿은 경우, 생애주기 종료를 야기하는 양의 데미지를 적용하는 컴포넌트
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(EggLifecycleHandler))]
public class EggNestAreaDamageSource : EggDamageSourceBase
{

#region UnityCallbacks

	protected override void Awake()
	{
		base.Awake();

		// Cache components

		_eggLifecycleHandler = GetComponent<EggLifecycleHandler>();
	}

#endregion // UnityCallbacks

#region UnityCollision

	void OnTriggerEnter(Collider collider)
	{
		// Is this actually a border of nest?
		if (collider.gameObject.layer != _nestEggDeadZoneLayer)
		{
			return;
		}

		// Am I allowed to cross the border of nest?
		if (!_eggLifecycleHandler.IsNestEgg)
		{
			return;
		}

		// You DIE!!!
		ForceInflictLethalDamage();
	}

#endregion // UnityCollision

	EggLifecycleHandler _eggLifecycleHandler;
	readonly int _nestEggDeadZoneLayer = 10;

}

}
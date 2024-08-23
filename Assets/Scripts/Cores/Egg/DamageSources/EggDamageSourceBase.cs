using UnityEngine;

namespace MC
{

/// <summary>
/// Egg의 체력에 영향주는 다양한 요인을 데미지 요인(DamageSource)라고 부르기로 한다. <br/>
/// 다음의 클래스는 구체적인 데미지 요인들이 공통적으로 가지고 있어야 하는 함수성을 규정한다.
/// </summary>
[RequireComponent(typeof(EggHealthManager))]
public abstract class EggDamageSourceBase : MonoBehaviour
{

#region UnityCallbacks

	protected virtual void Awake()
	{
		// Cache components

		_eggHealthManager = GetComponent<EggHealthManager>();
	}

#endregion // Unity Callbacks

	/// <summary>
	/// <see cref="EggHealthManger"/>에게 데미지가 발생하였음을 알린다.
	/// </summary>
	public virtual void CauseDamage(float damageAmount)
	{
		_eggHealthManager.TryInflictDamage(damageAmount);
	}

	public virtual void ForceInflictLethalDamage()
	{
		_eggHealthManager.InflictDamage(_eggHealthManager.MaxHealth);
	}

	EggHealthManager _eggHealthManager;
}

}
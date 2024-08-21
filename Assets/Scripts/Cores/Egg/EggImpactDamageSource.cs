using UnityEngine;

namespace MC
{

/// <summary>
/// Egg에 가해지는 충격이 임계값을 넘으면 데미지를 적용해야 함을 알린다. <br/>
/// 데미지 요인 중 하나이다.
/// </summary>
[DisallowMultipleComponent]
public partial class EggImpactDamageSource : EggDamageSourceBase
{
	public delegate void ImpactedHandler(in Vector3 impact, in bool isOverThreshold);
	/// <summary>
	/// Egg가 충돌했을 때, 그 힘에 대해서 알고싶다면 이것을 구독한다.
	/// </summary>
	public event ImpactedHandler Impacted;

#region UnityCollisions

	/// <summary>
	/// 전달된 충격량이 임계값을 넘어설 때, 어느 정도의 데미지를 받아야 하는지 알린다. <br/>
	/// 임계값을 넘지 못하면, 아무 일도 일어나지 않는다.
	/// </summary>
	void OnCollisionEnter(Collision collision)
	{
		var impact = collision.impulse;
		var isOverThreshold = IsOverThreshold(impact);

		Impacted?.Invoke(impact, isOverThreshold);

		if (!IsOverThreshold(impact))
		{
			return;
		}

		var damageAmount = ConvertImpactToDamage(impact);

		// Try inflict damage
		CauseDamage(damageAmount);
	}

#endregion // UnityCollisions

	bool IsOverThreshold(in Vector3 impact) => impact.sqrMagnitude > _impactMagnitudeThreshold * _impactMagnitudeThreshold;

	// TODO 받은 충격량을 데미지로 변환하는 자세한 공식은 여기에서 작성한다.
	float ConvertImpactToDamage(in Vector3 impact) => impact.magnitude * 2.0f;

	[SerializeField] float _impactMagnitudeThreshold = 1.0f;
}

}
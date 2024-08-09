using UnityEngine;

namespace MC
{

/// <summary>
/// Egg에 가해지는 충격이 임계값을 넘으면 데미지로 변환하여,
/// 데미지를 받아야 함을 알린다.
/// </summary>
[DisallowMultipleComponent]
public partial class EggImpactDetector : MonoBehaviour
{
	public delegate void ImpactedEventHandler(in Vector3 impact);
	public event ImpactedEventHandler Impacted;
	public delegate void ShouldInflictDamageEventHandler(in float damage);
	public event ShouldInflictDamageEventHandler ShouldInflictDamage;

#region UnityCallbacks

	void Awake()
	{
		// Bind events

		Impacted += OnImpacted;
	}

	void OnDestroy()
	{
		// Unbind events

		Impacted -= OnImpacted;
	}

#endregion // UnityCallbacks

#region UnityCollisions

	void OnCollisionEnter(Collision collision)
	{
		Impacted?.Invoke(collision.impulse);
	}

#endregion // UnityCollisions

	/// <summary>
	/// 전달된 충격량이 임계값을 넘어설 때, 어느 정도의 데미지를 받아야 하는지 알린다. <br/>
	/// 임계값을 넘지 못하면, 아무 일도 일어나지 않는다.
	/// </summary>
	void OnImpacted(in Vector3 impact)
	{
		if (!IsOverThreshold(impact))
		{
			return;
		}

		ShouldInflictDamage?.Invoke(ConvertImpactToDamage(impact));
	}

	bool IsOverThreshold(in Vector3 impact) => impact.sqrMagnitude > _impactMagnitudeThreshold * _impactMagnitudeThreshold;

	/// <remarks>
	/// TODO 받은 충격량을 데미지로 변환하는 자세한 공식은 여기에서 작성한다.
	/// </remarks>
	float ConvertImpactToDamage(in Vector3 impact)
	{
		return impact.magnitude * 2.0f;
	}

	[SerializeField] float _impactMagnitudeThreshold = 1.0f;
}

}
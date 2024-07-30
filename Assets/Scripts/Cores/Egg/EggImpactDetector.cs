using UnityEngine;

namespace MC
{
	/// <summary>
	/// 달걀에 가해지는 충격이 임계값을 넘으면 데미지로 변환하여,
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
			var impact = collision.impulse;

			Impacted?.Invoke
			(
				impact: impact
			);

#if UNITY_EDITOR
			_lastImpact = impact;
#endif
		}

		#endregion // UnityCollisions

		/// <summary>
		/// 전달된 힘이 임계값 이상이라면 데미지를 받아야 함을 Notify
		/// </summary>
		void OnImpacted(in Vector3 impact)
		{
			if (!IsOverThreshold(impact))
			{
				return;
			}

			ShouldInflictDamage?.Invoke
			(
				damage: ConvertImpactToDamage(impact)
			);
		}

		bool IsOverThreshold(in Vector3 impact)
		{
			return impact.sqrMagnitude > _impactMagnitudeThreshold * _impactMagnitudeThreshold;
		}

		/// <remarks>
		/// 받은 충격량을 데미지로 변환하는 공식은 여기에서 작성한다.
		/// </remarks>
		float ConvertImpactToDamage(in Vector3 impact)
		{
			return impact.magnitude * 2.0f;
		}

		[SerializeField] float _impactMagnitudeThreshold = 1.0f;
	}
}
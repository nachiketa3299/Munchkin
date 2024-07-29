using UnityEngine;

namespace MC
{
	/// <summary>
	/// Egg에 가해지는 외력이 임계값을 넘으면 신호를 보냄
	/// </summary>
	[DisallowMultipleComponent]
	public partial class EggImpactDetector : MonoBehaviour
	{
		delegate void EggImpactEventHandler(in float impactForceMagnitude);
		event EggImpactEventHandler EggImpacted;
		public delegate void EggShouldDamagedEventHandler(in float impactForceMagnitude);
		public event EggShouldDamagedEventHandler ImpactCrossedThreshold;

		#region Unity Callbacks

		void OnEnable()
		{
			EggImpacted += OnEggImpacted;
		}

		void OnCollisionEnter(Collision collision)
		{
			var impactForce = collision.impulse;
			var impactForceMagnitude = impactForce.magnitude;

			EggImpacted?.Invoke(impactForceMagnitude);

#if UNITY_EDITOR
			_lastImpactForce = impactForce;
#endif
		}

		void OnDisable()
		{
			EggImpacted -= OnEggImpacted;
		}


		#endregion // Unity Callbacks

		/// <summary>
		/// 전달된 힘이 임계값 이상이라면 데미지를 받아야 함을 Notify
		/// </summary>
		void OnEggImpacted(in float impactForceMagnitude)
		{
			if (impactForceMagnitude <= _impactForceMagnitudeThreshold)
			{
				return;
			}

			ImpactCrossedThreshold?.Invoke(impactForceMagnitude);
		}

		[SerializeField] float _impactForceMagnitudeThreshold = 10.0f;
		LayerMask _characterLayerMask = 1 << 7;
	}
}
using UnityEngine;

namespace MC
{
	// 어쩌면 필요 없을 수도 있다.
	[CreateAssetMenu(menuName = "MC/Scriptable Objects/Move Action Data", fileName = "MoveActionData_XX")]
	public class MoveActionData : ScriptableObject
	{
		// AccelerationOnGround 로 쓰는것 보다는 AccMagOnGround 로 쓰는게 나을 것 같아서 축약어 사용
		public float AccMagOnGround => _onGround.acceleration;
		public float DecMagOnGround => _onGround.deceleration;
		public float AccMagOnAir => _onAir.acceleration;
		public float DecMagOnAir => _onAir.deceleration;

		/// <summary>
		/// Acceleration & Deceleration Magnitude
		/// </summary>
		[System.Serializable]
		private struct AccDecMag
		{
			public float acceleration;
			public float deceleration;
		}

		[SerializeField] AccDecMag _onAir = new() { acceleration = 8.0f, deceleration = 8.0f };
		[SerializeField] AccDecMag _onGround = new() { acceleration = 8.0f, deceleration = 8.0f };
	}
}


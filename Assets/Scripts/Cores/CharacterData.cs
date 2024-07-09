using UnityEngine;

namespace MC
{
	/// <summary> 개별 캐릭터의 상세한 정보를 저장하는 스크립터블 오브젝트 </summary>
	[CreateAssetMenu(fileName = "CharacterData_XX", menuName = "MC/Scriptable Objects/Character Data")]
	public class CharacterData : ScriptableObject
	{
		/// <summary> 한 캐릭터 내에서 노화에 따라 색이 어디에서 어디로 변할지 저장하기 위한 용도 </summary>
		[System.Serializable]
		public class AgingColors
		{
			public Color start;
			public Color end;
		} // inner class

		/// <summary> 캐릭터별 이동 관련 스탯을 저장하기 위한 용도</summary>
		[System.Serializable]
		public class MovementStats
		{
			public float accelerationMagnitudeOnGround;
			public float accelerationMagnitudeOnAir;
			public float decelerationMagnitudeOnGround;
			public float decelerationMagnitudeOnAir;
		} // inner class

		[HideInInspector] public GameObject visualInstance = null;
		[HideInInspector] public Renderer visualRenderer = null;

		// 현재 테스트로 임포트한 스킨드 메쉬들의 방향과 Y 오프셋이 약간 어긋나 있어서, 아래 값들로 보정하여 인스턴스화 한다.
		static public Vector3 PosCal => new(0.0f, -1.0f, 0.0f);
		static public Quaternion RotCal => Quaternion.identity * Quaternion.Euler(0.0f, 180.0f, 0.0f);

		#region Assign in Inspector

		public GameObject visualPrefab; // WARNING 여기에 Skinned Mesh 가 아닌 다른 걸 할당하는 것을 막을 방법이 없음
		public AgingColors agingColors;
		public MovementStats movementStats;

		#endregion // Assign in Inspector
	} // class
} // namespace
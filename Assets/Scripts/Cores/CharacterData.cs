using Codice.CM.Common;
using UnityEngine;

namespace MC
{
	/// <summary>
	/// 개별 캐릭터의 정보를 저장하는 스크립터블 오브젝트
	/// </summary>
	[CreateAssetMenu(fileName = "CharacterData_XX", menuName = "MC/Scriptable Objects/Character Data")]
	public class CharacterData : ScriptableObject
	{
		[System.Serializable]
		public class AgingColors
		{
			public Color start;
			public Color end;
		}

		[System.Serializable]
		public class MovementStats
		{
			public float accelerationMagnitudeOnGround;
			public float accelerationMagnitudeOnAir;
			public float decelerationMagnitudeOnGround;
			public float decelerationMagnitudeOnAir;
		}

		public GameObject visualPrefab; // 여기에 Skinned Mesh 가 아닌 다른 걸 할당하는 것을 막을 방법이 없음
		[HideInInspector] public GameObject visualInstance = null;
		[HideInInspector] public Renderer visualRenderer = null;
		public AgingColors agingColors;
		public MovementStats movementStats;

		static public Vector3 PosCal => new(0.0f, -1.0f, 0.0f);
		static public Quaternion RotCal => Quaternion.identity * Quaternion.Euler(0.0f, -90.0f, 0.0f);
	}

}
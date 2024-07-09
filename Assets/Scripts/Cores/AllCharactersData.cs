using UnityEngine;

namespace MC
{
	/// <summary>
	/// 게임에 등장하는 모든 캐릭터들의 데이터를 한꺼번에 가지고 있는 스크립터블 오브젝트. <br/>
	/// 개별 캐릭터 데이터에 대해서는 <see cref="CharacterData"/>를 참고한다.
	/// </summary>
	[CreateAssetMenu(fileName = "AllCharactersData_TestChicken", menuName = "MC/Scriptable Objects/All Characters Data")]
	public class AllCharactersData : ScriptableObject
	{
		public void InitializeVisualInstance(ECharacterType type, Transform parent)
		{
			if (!this[type].visualInstance)
			{
				this[type].visualInstance = Instantiate(this[type].visualPrefab, parent.position + CharacterData.PosCal, parent.rotation * CharacterData.RotCal, parent);
				this[type].visualInstance.SetActive(false);
				this[type].visualRenderer = this[type].visualInstance.GetComponentInChildren<Renderer>();
			}
		}

		public Renderer GetVisualRenderer(ECharacterType type) => this[type].visualRenderer;
		public GameObject GetVisualInstance(ECharacterType type) => this[type].visualInstance;
		// public CharacterData.AgingColors GetAgingColors(ECharacterType type) => this[type].agingColors;
		// public CharacterData.MovementStats GetMovementStats(ECharacterType type) => this[type].movementStats;

		public CharacterData this[ECharacterType type]
		{
			get
			{
				switch (type)
				{
					case ECharacterType.Hen:
						return henData;
					case ECharacterType.Rooster:
						return roosterData;
					case ECharacterType.Soul:
						return soulData;
					case ECharacterType.Chick:
					default:
						return chickData;
				}
			}
		}

		public CharacterData chickData;
		public CharacterData henData;
		public CharacterData roosterData;
		public CharacterData soulData;

	}
}
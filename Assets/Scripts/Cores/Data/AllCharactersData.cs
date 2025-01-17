using UnityEngine;

namespace MC
{

/// <summary>
/// 게임에 등장하는 모든 캐릭터들의 데이터를 한꺼번에 가지고 있는 스크립터블 오브젝트. <br/>
/// 개별 캐릭터 데이터에 대해서는 <see cref="CharacterData"/>를 참고한다. <br/>
/// 이를 주로 사용하는 곳은 <see cref="MutationHandler"/> 이다.
/// </summary>
/// <remarks>
/// 런타임 에서는 캐릭터 비주얼 모델의 인스턴스에 대한 레퍼런스를 가지고 있게 된다.
/// </remarks>
[CreateAssetMenu(fileName = "AllCharactersData_TestChicken", menuName = "MC/Scriptable Objects/All Characters Data")]
public class AllCharactersData : ScriptableObject
{
	/// <summary>
	/// <paramref name="type"/> 에 해당하는 캐릭터 오브젝트를 인스턴스화하고, <paramref name="parent"/>의 자식으로 붙인다.
	/// </summary>
	/// <remarks> 이미 인스턴스가 생성되어 있는 경우에는 무시한다. </remarks>
	public void InitializeVisualInstance(ECharacterType type, Transform parent)
	{
		if (!this[type].visualInstance)
		{
			this[type].visualInstance =
				Instantiate(this[type].visualPrefab, parent);
			this[type].visualInstance.SetActive(false);
			this[type].visualRenderer = this[type].visualInstance.GetComponentInChildren<Renderer>();
		}
	}

	public Renderer GetVisualRenderer(ECharacterType type) => this[type].visualRenderer;
	public GameObject GetVisualInstance(ECharacterType type) => this[type].visualInstance;

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
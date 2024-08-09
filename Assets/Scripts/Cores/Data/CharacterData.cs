using UnityEngine;

namespace MC
{

/// <summary>
/// 개별 캐릭터의 상세한 정보를 저장하는 스크립터블 오브젝트
/// </summary>
[CreateAssetMenu(fileName = "CharacterData_XX", menuName = "MC/Scriptable Objects/Character Data")]
public class CharacterData : ScriptableObject
{
	/// <summary>
	/// 한 캐릭터 내에서 노화에 따라 색이 어디에서 어디로 변할지 저장하기 위한 용도
	/// </summary>
	[System.Serializable]
	public class AgingColors
	{
		public Color start;
		public Color end;
	} // inner class

	[HideInInspector] public GameObject visualInstance = null;
	[HideInInspector] public Renderer visualRenderer = null;
	public GameObject visualPrefab;
	public AgingColors agingColors;
}

}
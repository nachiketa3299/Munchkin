using System;

using UnityEngine;

namespace MC
{

// TODO Aging의 경우 따로 컴포넌트를 만들어야 할 수도 있음.
/// <summary>
/// 캐릭터의 변이(hen -> chicken) / 노화(in character) / 사망(character -> soul) / 부활(soul -> hen) 시점에서
/// 캐릭터의 변경을 담당한다. <br/>
/// 여기서 캐릭터의 변경은 외형적인 것 뿐만아니라 스탯 등의 내부 속성도 포함하기로 한다. (프리팹으로 관리)
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(LifespanHandler))]
public class MutationHandler : MonoBehaviour
{
	public event Action<ECharacterType> Mutated;

	#region UnityCallbacks

	void Awake()
	{
		// Cache components & data

		_lifespanHandler = GetComponent<LifespanHandler>();

#if UNITY_EDITOR
		if (!_allCharactersData)
		{
			Debug.LogWarning("AllCharactersData를 찾을 수 없습니다.");
		}
#endif

		// Instantiate visual instances

		foreach (ECharacterType type in ECharacterType.GetValues(typeof(ECharacterType)))
		{
			_allCharactersData.InitializeVisualInstance(type, transform);
		}
	}

	void OnEnable()
	{
		_lifespanHandler.Started += InitializeVisualInstance;
		_lifespanHandler.ReachedMutationThreshold += Mutate;
		_lifespanHandler.Changed += ChangeVisualInstanceColor;
		_lifespanHandler.Ended += OnLifespanEnded;
	}

	void OnDestroy()
	{
		_lifespanHandler.Started -= InitializeVisualInstance;
		_lifespanHandler.ReachedMutationThreshold -= Mutate;
		_lifespanHandler.Changed -= ChangeVisualInstanceColor;
		_lifespanHandler.Ended -= OnLifespanEnded;
	}

	#endregion // UnityCallbacks

	void InitializeVisualInstance()
	{
		_currentVisualInstance = _allCharactersData.GetVisualInstance(ECharacterType.Chick);

		_currentVisualInstance.SetActive(true);
		_currentCharacterType = ECharacterType.Chick;
	}

	void Mutate()
	{
		_currentVisualInstance.SetActive(false);

		if (UnityEngine.Random.Range(0f, 1f) < _roosterMutationChance)
		{
			_currentVisualInstance = _allCharactersData.GetVisualInstance(ECharacterType.Rooster);
			_currentCharacterType = ECharacterType.Rooster;
		}
		else
		{
			_currentVisualInstance = _allCharactersData.GetVisualInstance(ECharacterType.Hen);
			_currentCharacterType = ECharacterType.Hen;
		}

		Mutated?.Invoke(_currentCharacterType);

		_currentVisualInstance.SetActive(true);
	}

	void ChangeVisualInstanceColor(in float totalRatio, in float currentRatio)
	{
		var colorPalette = _allCharactersData[_currentCharacterType].agingColors;
		var agingColor = Color.Lerp(colorPalette.start, colorPalette.end, currentRatio);

		_allCharactersData.GetVisualRenderer(_currentCharacterType).material.color = agingColor;
	}

	void OnLifespanEnded()
	{
		// TODO 영혼이 되었을 때의 로직을 여기서 작성
		_currentVisualInstance.SetActive(false);
	}

	LifespanHandler _lifespanHandler;
	[SerializeField][HideInInspector] GameObject _currentVisualInstance = null;
	[SerializeField][HideInInspector] ECharacterType _currentCharacterType;
	[SerializeField] AllCharactersData _allCharactersData;
	[SerializeField][Range(0f, 1f)] float _roosterMutationChance = 0.05f;
}

} // namespace

using System;

using UnityEngine;

namespace MC
{

// TODO Aging의 경우 따로 컴포넌트를 만들어야 할 수도 있음.
/// <summary>
/// 캐릭터의 변이시점에서 실제로 일어나야 하는 일들을 일어나도록 한다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(LifespanHandler))]
public class MutationHandler : MonoBehaviour
{
	public delegate void MutatedHandler(ECharacterType characterType);
	public event MutatedHandler Mutated;

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

		// Hen 으로 변이
		if (UnityEngine.Random.Range(0f, 1f) < _roosterMutationChance)
		{
			_currentVisualInstance = _allCharactersData.GetVisualInstance(ECharacterType.Rooster);
			_currentCharacterType = ECharacterType.Rooster;
		}
		// Rooster 로 변이
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

}

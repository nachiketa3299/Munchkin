using UnityEngine;
using UnityEngine.Assertions;

namespace MC
{
	// TODO Aging의 경우 따로 컴포넌트를 만들어야 할 수도 있음.
	/// <summary>
	/// 캐릭터의 변이(hen -> chicken) / 노화(in character) / 사망(character -> soul) / 부활(soul -> hen) 시점에서
	/// 캐릭터의 변경을 담당한다. <br/>
	/// 여기서 캐릭터의 변경은 외형적인 것 뿐만아니라 스탯 등의 내부 속성도 포함하기로 한다.
	/// </summary>
	[RequireComponent(typeof(LifespanHandler))]
	[DisallowMultipleComponent]
	public class MutationHandler : MonoBehaviour
	{
		#region Unity Messages

		void Awake()
		{
			_lifespanHandler = GetComponent<LifespanHandler>();

			Assert.IsNotNull(_allCharacterData);

			foreach (ECharacterType type in ECharacterType.GetValues(typeof(ECharacterType)))
			{
				_allCharacterData.InitializeVisualInstance(type, transform);
			}
		}

		void OnEnable()
		{
			LifespanHandler.LifespanStarted += OnLifespanStarted;
			LifespanHandler.LifespanReachedMutationThreshold += OnLifespanReachedMutationThreshold;
			LifespanHandler.LifespanChanged += OnLifespanChanged;
			LifespanHandler.LifespanEnded += OnLifespanEnded;
		}

		void OnDisable()
		{
			LifespanHandler.LifespanStarted -= OnLifespanStarted;
			LifespanHandler.LifespanReachedMutationThreshold -= OnLifespanReachedMutationThreshold;
			LifespanHandler.LifespanChanged -= OnLifespanChanged;
			LifespanHandler.LifespanEnded -= OnLifespanEnded;
		}

		#endregion // Unity Messages

		void OnLifespanStarted()
		{
			_currentVisualInstance = _allCharacterData.GetVisualInstance(ECharacterType.Chick);
			RefreshAgingColors(ECharacterType.Chick);
			_currentVisualInstance.SetActive(true);
			_currentCharacterType = ECharacterType.Chick;

			_isMutated = false;
		}

		void OnLifespanReachedMutationThreshold()
		{
			_currentVisualInstance.SetActive(false);

			if (Random.Range(0f, 1f) < _roosterMutationChance)
			{
				_currentVisualInstance = _allCharacterData.GetVisualInstance(ECharacterType.Rooster);
				RefreshAgingColors(ECharacterType.Rooster);
				_currentCharacterType = ECharacterType.Rooster;
			}
			else
			{
				_currentVisualInstance = _allCharacterData.GetVisualInstance(ECharacterType.Hen);
				RefreshAgingColors(ECharacterType.Hen);
				_currentCharacterType = ECharacterType.Hen;
			}

			_currentVisualInstance.SetActive(true);
			_isMutated = true;
		}

		void OnLifespanChanged(float totalSpanRatio, float agingRatio)
		{
			// NOTE 현재 Color 업데이트가 LifespanHandler의 resolution에 영향을 받음,
			// 수명이 긴 경우 (3분 이상) 티가 나지 않지만, 짧은 경우 각 컬러 업데이트가 끊기는 것이 보임.
			_currentAgingColor = Color.Lerp(_currentAgingStartColor, _currentAgingEndColor, agingRatio);
			_allCharacterData.GetVisualRenderer(_currentCharacterType).material.color = _currentAgingColor;
		}

		void OnLifespanEnded()
		{
			_currentVisualInstance.SetActive(false);
		}

		void RefreshAgingColors(ECharacterType type)
		{
			var palette = _allCharacterData[type].agingColors;
			_currentAgingStartColor = palette.start;
			_currentAgingEndColor = palette.end;
			_currentAgingColor = palette.start;
		}

		GameObject _currentVisualInstance = null;
		ECharacterType _currentCharacterType;
		Color _currentAgingStartColor;
		Color _currentAgingColor;
		Color _currentAgingEndColor;

		bool _isMutated;

		[SerializeField] AllCharactersData _allCharacterData;
		[SerializeField][Range(0f, 1f)] float _roosterMutationChance = 0.05f;

		LifespanHandler _lifespanHandler;
	}
} // namespace

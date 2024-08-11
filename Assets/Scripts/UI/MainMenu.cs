using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace MC.UI
{

/// <summary> 메인 메뉴에서 특정하게 발생하는 사건들을 처리하기 위한 스크립트 </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour
{

#region UnityCallbacks

	// UI 설정은 OnEnable에서 이루어져야 한다고 함.
	void OnEnable()
	{
		// find all ui elements
		var root = GetComponent<UIDocument>().rootVisualElement;

		_ui_gameStartButton = root.Q<Button>("GameStartButton");
		_ui_gameEndButton = root.Q<Button>("GameEndButton");

		_ui_mainInteraction = root.Q<VisualElement>("MainInteraction");
		_ui_progress = root.Q<ProgressBar>("ProgressBar");

		// set ui element
		_ui_mainInteraction.style.display = DisplayStyle.Flex;
		_ui_progress.style.display = DisplayStyle.None;
		_ui_progress.value = 0.0f;

		// set button events
		_ui_gameStartButton.clicked += OnGameStartButtonClicked;
		_ui_gameEndButton.clicked += OnGameEndButtonClicked;

		_ui_title = root.Q<Label>("Title");

		// set title init color
		StartCoroutine(TitleColorTransitionLoopRoutine());
	}

#endregion // UnityCallbacks

	void OnGameStartButtonClicked()
	{
		_ui_mainInteraction.style.display = DisplayStyle.None;
		_ui_progress.style.display = DisplayStyle.Flex;

		StartCoroutine(InitialGameLoadingRoutine());
	}

	void OnGameEndButtonClicked()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}

	/// <summary> 설정된 최초 씬과 PersistentGameplay 씬을 로드하는 코루틴 </summary>
	IEnumerator InitialGameLoadingRoutine()
	{
		// turn off main ui and turn on loading screen
		_ui_mainInteraction.style.display = DisplayStyle.None;
		_ui_progress.style.display = DisplayStyle.Flex;

		var initialScenes = new string[] { _data.PersistentSceneName, _data.InitialSceneName };

		var operations = new List<AsyncOperation>();

		foreach (var name in initialScenes)
		{
			if (SceneManager.GetSceneByName(name).isLoaded)
			{
				continue;
			}

			operations.Add(SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive));
		}

		while (!operations.All(operation => operation?.isDone == true))
		{
			var progress = operations.Aggregate(0.0f, (acc, op) => acc + op.progress) / operations.Count;
			_ui_progress.value = progress;
			yield return null;
		}

		// unload this scene and destroy all!
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(_data.PersistentSceneName)); // 안하면 안 됨 (왜 안되는지궁금하면 저에게 질문하세용)
		SceneManager.UnloadSceneAsync(gameObject.scene.name);
	}

	IEnumerator TitleColorTransitionLoopRoutine()
	{
		var currentIndex = 0;

		while (true)
		{
			var startColor = _titleColors[currentIndex];
			var targetColor = _titleColors[(currentIndex + 1) % _titleColors.Count];

			for (var t = 0.0f; t < _titleColorTransitionDuration; t += Time.deltaTime)
			{
				var lerpedColor = Color.Lerp(startColor, targetColor, t / _titleColorTransitionDuration);
				_ui_title.style.color = new StyleColor(lerpedColor);
				yield return null;
			}

			currentIndex = (currentIndex + 1) % _titleColors.Count;
		}
	}

	[SerializeField] SceneDependencyData _data;

	VisualElement _ui_mainInteraction;
	ProgressBar _ui_progress;
	Button _ui_gameStartButton;
	Button _ui_gameEndButton;
	Label _ui_title;

	// NOTE 시작 색을 아예 여기서 설정해주고 있는데, Custom USS Property 에 대해서 VisualElement 클래스를 새로 상속해서
	// Getter 를 만드는 방법이 있다고 하는데 그걸 쓰느니 이렇게 여기서 그냥 색을 정해버리는 게 빠를 거 같아서 이렇게함
	// 참고: https://forum.unity.com/threads/is-getting-values-from-style-sheet-possible-in-c.1399723/
	List<Color> _titleColors = new() { new Color(255f / 256f, 211f / 256f, 0f, 1f), Color.white, Color.cyan };
	[SerializeField] float _titleColorTransitionDuration = 3.0f;
}

}
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
		#region Unity Messages

		// UI 설정은 OnEnable에서 이루어져야 한다고 함.
		void OnEnable()
		{
			// find all ui elements
			var root = GetComponent<UIDocument>().rootVisualElement;

			_ui_gameStartButton = root.Q<Button>("GameStartButton");

			_ui_mainInteraction = root.Q<VisualElement>("MainInteraction");
			_ui_progress = root.Q<ProgressBar>("ProgressBar");

			// set ui element
			_ui_mainInteraction.style.display = DisplayStyle.Flex;
			_ui_progress.style.display = DisplayStyle.None;
			_ui_progress.value = 0.0f;

			_ui_gameStartButton.clicked += OnGameStartButtonClicked;
		}

		#endregion // Unity Messages

		void OnGameStartButtonClicked()
		{
			_ui_mainInteraction.style.display = DisplayStyle.None;
			_ui_progress.style.display = DisplayStyle.Flex;


			StartCoroutine(InitialGameLoadingRoutine());
		}

		/// <summary> 설정된 최초 씬과 PersisteneGameplay 씬을 로드하는 코루틴 </summary>
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

		[SerializeField] SceneDepenencyData _data;

		VisualElement _ui_mainInteraction;
		ProgressBar _ui_progress;
		Button _ui_gameStartButton;
	}
}
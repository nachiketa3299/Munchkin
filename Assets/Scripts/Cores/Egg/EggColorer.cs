using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public class EggColorer : MonoBehaviour
	{
		#region UnityCallbacks

		void Awake()
		{
			// Cache components

			_renderer = GetComponentInChildren<Renderer>();

#if UNITY_EDITOR
			if (!_renderer)
			{
				Debug.LogWarning("Renderer 컴포넌트를 찾을 수 없습니다.");
			}
#endif

			_eggHealthManager = GetComponent<EggHealthManager>();

#if UNITY_EDITOR
			if (!_eggHealthManager)
			{
				Debug.LogWarning("EggHealthManager 컴포넌트를 찾을 수 없습니다.");
			}
#endif

			// Bind events

			_eggHealthManager.HealthChanged += ChangeColor;
		}

		void OnDestroy()
		{
			// Unbind events

			_eggHealthManager.HealthChanged -= ChangeColor;
		}

		#endregion // UnityCallbacks

		void ChangeColor(in float healthRatio)
		{
			_currentColor = Color.Lerp
			(
				a: _colorOnZeroHealth,
				b: _colorOnMaxHealth,
				t: healthRatio
			);

			_renderer.material.color = _currentColor;
		}

		Renderer _renderer;
		EggHealthManager _eggHealthManager;
		Color _currentColor;
		Color _colorOnZeroHealth = Color.red;
		Color _colorOnMaxHealth = Color.white;
	}
}
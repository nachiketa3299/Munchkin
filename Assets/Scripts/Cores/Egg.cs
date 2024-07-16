using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MC
{
	// 스크립트가 뚱뚱해지면 분할해야 할 수도 있음.
	// 현재는 그냥 여기서 계속 처리하는 것으로
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public partial class Egg : MonoBehaviour, IPickable
	{
		public event Action EggCreated;
		public event Action<float> EggDamaged;
		public event Action<float> EggImpacted;
		public event Action EggDestroyed;

		public bool isPicked = false;

		#region Unity Messages

		void Awake()
		{
			_renderer = GetComponentInChildren<Renderer>();

			EggCreated += OnEggCreated;
			EggImpacted += OnEggImpacted;
			EggDamaged += OnEggDamaged;
			EggDestroyed += OnEggDestroyed;
		}

		void OnEnable()
		{
			// "EggCreate" means activate egg
			EggCreated?.Invoke();
		}

		void OnDisable()
		{
			// "EggDestroy" means deactivate egg
		}

		void OnDestroy()
		{
			// nuke all events
			EggCreated = null;
			EggImpacted = null;
			EggDamaged = null;
			EggDestroyed = null;
		}

		void OnCollisionEnter(Collision collision)
		{
			var impactForceMagnitude = (collision.impulse / Time.fixedDeltaTime).magnitude;
			EggImpacted?.Invoke(impactForceMagnitude);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.tag.Equals("MC_Scene"))
			{
				string sceneName = other.gameObject.scene.name;
				Scene moveToScene = SceneManager.GetSceneByName(sceneName);
				SceneManager.MoveGameObjectToScene(this.gameObject, moveToScene);
			}
		}

		#endregion // Unity Messages

		void OnEggCreated()
		{
			_currentHealth = _maxHealth;
			TryUpdateColorByHealth();
		}

		void OnEggImpacted(float impactForceMagnitude)
		{
			_lastImpactForce = impactForceMagnitude;

			if (impactForceMagnitude < _impactThreshold)
			{
				return;
			}

			if (!_eggCanBeDamaged)
			{
				return;
			}

			EggDamaged?.Invoke(ForceToHealth(impactForceMagnitude));
		}

		void OnEggDamaged(float damage)
		{
			_currentHealth -= damage;
			TryUpdateColorByHealth();

			if (_currentHealth <= 0.0f)
			{
				EggDestroyed?.Invoke();
				return;
			}

			StartCoroutine(EggDamageTimer());
		}

		IEnumerator EggDamageTimer()
		{
			_eggDamageTimerElapsedTime = 0.0f;
			_eggCanBeDamaged = false;

			while (_eggDamageTimerElapsedTime < _eggDamageTimer)
			{
				_eggDamageTimerElapsedTime += Time.deltaTime;
				yield return null;
			}

			_eggCanBeDamaged = true;
			_eggDamageTimerElapsedTime = 0.0f;
		}

		void OnEggDestroyed()
		{
			gameObject.SetActive(false);
		}

		void TryUpdateColorByHealth()
		{
			var colorRatio = _colorCurve.Evaluate(HealthRatio);
			_currentColor = Color.Lerp(_colorOnZeroHealth, _colorOnMaxHealth, colorRatio);
			_renderer.material.color = _currentColor;
		}

		float ForceToHealth(float forceMagnitude)
		{
			return 0.02f * forceMagnitude;
		}

		Renderer _renderer;

		float ImpactRatio => _lastImpactForce / _impactThreshold;
		[SerializeField] float _impactThreshold = 10.0f;
		float _lastImpactForce = 0.0f;

		public float HealthRatio => _currentHealth / _maxHealth;
		float _currentHealth;
		[SerializeField] float _maxHealth = 100.0f;

		Color _currentColor;

		// 아마 Scriptable Object EggData 같은걸 만들어서 관리해야 할 수도 있음
		[SerializeField] Color _colorOnMaxHealth = Color.yellow;
		[SerializeField] Color _colorOnZeroHealth = Color.red;
		[SerializeField] AnimationCurve _colorCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

		[SerializeField] float _eggDamageTimer = 5.0f;
		float _eggDamageTimerElapsedTime = 0.0f;
		float EggDamageTimerRatio => _eggDamageTimerElapsedTime / _eggDamageTimer;
		bool _eggCanBeDamaged = true;

#if UNITY_EDITOR
		// bool _logOnEggEvent = false;
		private IPickable _pickableImplementation; // why?
#endif
		public void Pick()
		{
			isPicked = true;
		}

		public void Drop()
		{
			isPicked = false;
		}
	}
}
using System;
using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public class Egg : MonoBehaviour
	{
		public event Action EggCreated;
		public event Action EggDamaged;
		public event Action EggDestroyed;

		#region Unity Messages

		void Awake()
		{
			_renderer = GetComponentInChildren<Renderer>();
		}

		#endregion // Unity Messages

		Renderer _renderer;

		float _currentHealth;
		[SerializeField] float _maxHealth = 100f;
	}
}
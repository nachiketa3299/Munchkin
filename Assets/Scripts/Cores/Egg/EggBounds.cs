using System.Linq;

using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public class EggBounds : MonoBehaviour
	{
		#region Unity Callbacks
		void Awake()
		{
			_colliders = GetComponentsInChildren<Collider>().Where(collider => !collider.isTrigger).ToArray();

#if UNITY_EDITOR
			if (_colliders.Length == 0)
			{
				Debug.LogWarning("아무런 피지컬 콜라이더가 발견되지 않았습니다.");
			}
#endif

			_bounds = _colliders[0].bounds;
			for (var i = 1; i < _colliders.Length; ++i)
			{
				_bounds.Encapsulate(_colliders[i].bounds);
			}
		}

		#endregion

		public Bounds CombinedBounds => _bounds;
		Collider[] _colliders;
		Bounds _bounds;
	}
}
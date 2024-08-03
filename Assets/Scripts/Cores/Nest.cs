using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public class Nest : MonoBehaviour
	{
		#region UnityCallbacks

		void Awake()
		{
#if UNITY_EDITOR
			if (!_eggFactory)
			{
				Debug.LogWarning("EggFactory 컴포넌트를 찾을 수 없습니다.");
			}
#endif
		}

		#endregion // UnityCallbacks

		EggLifecycleHandler _currentNestEgg;
		EggFactory _eggFactory;
	}
}
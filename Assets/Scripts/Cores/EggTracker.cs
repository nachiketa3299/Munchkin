using UnityEngine;

namespace MC
{

[DisallowMultipleComponent]
public class EggTracker : MonoBehaviour
{
	#region UnityCallbacks

	void Awake()
	{
#if UNITY_EDITOR
		if (!_runtimePooledEggData)
		{
			Debug.Log("RuntimePooledEggData를 찾을 수 없습니다.");
		}
#endif
	}

	bool IsThereCharacterEgg()
	{
		return false;
	}

	#endregion // UnityCallbacks

	[SerializeField] RuntimePooledEggData _runtimePooledEggData;
}

}
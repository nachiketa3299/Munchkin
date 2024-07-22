using UnityEngine;
using UnityEngine.Assertions;

namespace MC
{
	[DisallowMultipleComponent]
	public partial class GrabAction : ActionRoutineBase
	{

		#region Unity Callbacks

		void Awake()
		{

#if UNITY_EDITOR
			if (!_grabSocket)
			{
				Debug.LogWarning("grabSocket에 아무것도 할당되지 않았습니다.");
			}
#endif

		}

		#endregion // Unity Callbacks

		public void BeginAction()
		{

#if UNITY_EDITOR
			Debug.Log("Grab Action Begin");
#endif

		}

		public void EndAction()
		{

#if UNITY_EDITOR
			Debug.Log("Grab Action End");
#endif

		}

		Collider[] _overlapResults;

		[SerializeField] LayerMask _grabObjectMask = 1 << 8;
		[SerializeField] GameObject _grabSocket;


#if UNITY_EDITOR
		bool _logOnGrabAction = false;
#endif
	}
}
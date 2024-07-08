using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public class LookAction : MonoBehaviour
	{
		void BeginAction(float directionCoeff)
		{ }

		void EndAction()
		{ }

		#region Unity Messages
		#endregion

		Coroutine _currentRoutine;
	}
} // namespace
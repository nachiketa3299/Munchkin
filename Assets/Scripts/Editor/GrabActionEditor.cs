#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC
{
	public partial class GrabThrowAction : ActionRoutineBase
	{
		void OnDrawGizmos()
		{
			if (!_grabThrowSocket)
			{
				return;
			}

			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(_grabThrowSocket.transform.position, 0.1f);
		}

		[CustomEditor(typeof(GrabThrowAction))]
		private class GrabActionEditor : Editor
		{
		}
	}
}

#endif


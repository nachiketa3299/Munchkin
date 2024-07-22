#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC
{
	public partial class GrabAction : ActionRoutineBase
	{
		void OnDrawGizmos()
		{
			if (!_grabSocket)
			{
				return;
			}

			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(_grabSocket.transform.position, 0.1f);
		}

		[CustomEditor(typeof(GrabAction))]
		private class GrabActionEditor : Editor
		{
		}
	}
}

#endif


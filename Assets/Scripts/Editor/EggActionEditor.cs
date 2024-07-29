#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace MC
{
	public partial class EggAction : ActionRoutineBase
	{

		void OnDrawGizmos()
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(_layPosition, 0.3f);
		}

		[CustomEditor(typeof(EggAction))]
		private class EggActionEditor : Editor
		{

		}
	}

}

#endif
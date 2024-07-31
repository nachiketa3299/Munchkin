#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace MC
{

	public partial class RuntimePooledEggData : ScriptableObject
	{
		public bool IsPoolInitialized => _pool != null;
		public int CountInactive => _pool.CountInactive;
		public int CountActive => _pool.CountActive;
		public int CountAll => _pool.CountAll;


		[CustomEditor(typeof(RuntimePooledEggData))]
		private class RuntimePooledEggDataEditor : Editor
		{
			#region UnityCallbacks

			void OnEnable()
			{
				_runtimePooledEggData = target as RuntimePooledEggData;
			}

			#endregion

			RuntimePooledEggData _runtimePooledEggData;
		}
	}
}

#endif
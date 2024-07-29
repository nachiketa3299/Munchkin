using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public partial class EggFactory : MonoBehaviour
	{
		/// <summary>
		/// 알을 지정된 위치와 회전에 스폰한다.
		/// </summary>
		public EggLifeCycleHandler Spawn(Vector3 position, Quaternion rotation, bool createdByPlayer = false)
		{
			var instance = _eggPool.Pool.Get();
			instance.transform.SetPositionAndRotation(position, rotation);

			return instance;
		}

		public void Despawn(EggLifeCycleHandler egg)
		{
			_eggPool.Pool.Release(egg);
		}

		[SerializeField] RuntimePooledEggData _eggPool;
	}
}
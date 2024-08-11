using System;

using UnityEngine;

namespace MC
{

	/// <summary>
	/// Egg가 파괴등의 이유로 운동 상태가 캐싱되어야 할 때 사용하는 데이터
	/// </summary>
	[Serializable]
	public struct FEggPhysicalState
	{
		public FEggPhysicalState(in EggLifecycleHandler eggLifecycleHandler)
		{
			lastPosition = eggLifecycleHandler.transform.position;
			lastRotation = eggLifecycleHandler.transform.rotation;

			lastVelocity = eggLifecycleHandler.Rigidbody.velocity;
			lastAngularVelocity = eggLifecycleHandler.Rigidbody.angularVelocity;
		}

		public FEggPhysicalState(in EggLifecycleHandler eggLifecycleHandler, in GrabThrowAction grabSubject)
		{
			lastPosition = eggLifecycleHandler.transform.position;
			lastRotation = eggLifecycleHandler.transform.rotation;

			lastVelocity = eggLifecycleHandler.Rigidbody.velocity + grabSubject.Rigidbody.velocity;
			lastAngularVelocity = eggLifecycleHandler.Rigidbody.angularVelocity + grabSubject.Rigidbody.velocity;
		}

		public Vector3 lastPosition;
		public Quaternion lastRotation;

		public Vector3 lastVelocity;
		public Vector3 lastAngularVelocity;
	}

}

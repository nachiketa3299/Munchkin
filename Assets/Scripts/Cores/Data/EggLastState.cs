using System;

using UnityEngine;

namespace MC
{
	/// <summary>
	/// Egg가 파괴등의 이유로 운동 상태가 캐싱되어야 할 때 사용하는 데이터
	/// </summary>
	[Serializable]
	public class EggLastState
	{
		public EggLastState(EggLifecycleHandler egg, GrabThrowAction grabber = null)
		{
			lastPosition = egg.transform.position;
			lastRotation = egg.transform.rotation;

			lastVelocity = egg.Velocity;
			lastAngularVelocity = egg.AngularVelocity;

			if (grabber != null)
			{
				lastVelocity += grabber.Velocity;
				lastAngularVelocity += grabber.AngularVelocity;
			}
		}

		public Vector3 lastPosition;
		public Quaternion lastRotation;

		public Vector3 lastVelocity;
		public Vector3 lastAngularVelocity;
	}
}
using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class GrabThrowTarget : MonoBehaviour
	{
		#region UnityCallbacks

		void Awake()
		{
			// Cache components

			_rigidbody = GetComponent<Rigidbody>();
		}

		#endregion // UnityCallbacks

		public void BeginGrabState(GameObject grabParent)
		{
			_rigidbody.isKinematic = true;
			_rigidbody.detectCollisions = false;

			transform.SetPositionAndRotation(grabParent.transform.position, Quaternion.identity);
			transform.SetParent(grabParent.transform);
		}

		public void EndGrabState()
		{
			_rigidbody.isKinematic = false;
			_rigidbody.detectCollisions = true;

			transform.SetParent(null);
		}

		/// <summary>
		/// Throw 를 수행하는 오브젝트의 마지막 Velocity와 Throw 행동의 힘을 더해, 물체에 힘을 가한다.
		/// </summary>
		public void Throw(in Vector3 lastThrowerVelocity, in Vector3 force)
		{
			_rigidbody.AddForce(lastThrowerVelocity, ForceMode.VelocityChange);
			_rigidbody.AddForce(force, ForceMode.Force);
		}

		Rigidbody _rigidbody;
	}
}
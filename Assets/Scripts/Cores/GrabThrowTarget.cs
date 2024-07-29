using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class GrabThrowTarget : MonoBehaviour
	{
		#region Unity Callbacks

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		#endregion

		public void BeginGrabState(GameObject grabParent)
		{
			_grabParent = grabParent;


			_rigidbody.isKinematic = true;
			_rigidbody.detectCollisions = false;

			transform.position = grabParent.transform.position;
			transform.rotation = Quaternion.identity;

			transform.parent = grabParent.transform;


			// _isGrabbed = true;
		}

		public void EndGrabState()
		{
			_grabParent = null;


			_rigidbody.isKinematic = false;
			_rigidbody.detectCollisions = true;

			transform.parent = null;

			// _isGrabbed = false;
		}

		public void Throw(in Vector3 lastThrowerVelocity, in Vector3 force)
		{
			_rigidbody.AddForce(lastThrowerVelocity, ForceMode.VelocityChange);
			_rigidbody.AddForce(force, ForceMode.Force);
		}

		Rigidbody _rigidbody;

		//bool _isGrabbed = false;
		GameObject _grabParent;
	}
}
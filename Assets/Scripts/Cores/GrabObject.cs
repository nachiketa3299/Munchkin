using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class GrabObject : MonoBehaviour
	{
		#region Unity Callbacks

		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		void FixedUpdate()
		{
			if (!_isGrabbed)
			{
				return;
			}

			if (!_grabParent)
			{
				return;
			}

			transform.position = _grabParent.transform.position;
			transform.rotation = _grabParent.transform.rotation;
		}

		#endregion

		public void BeginGrabState(GameObject grabParent)
		{
			_grabParent = grabParent;
			_rigidbody.isKinematic = true;

			transform.position = grabParent.transform.position;
			transform.parent = grabParent.transform;

			_isGrabbed = true;
		}

		public void EndGrabState()
		{
			_grabParent = null;
			_rigidbody.isKinematic = false;
			transform.parent = null;

			_isGrabbed = false;
		}

		Rigidbody _rigidbody;
		bool _isGrabbed = false;

		GameObject _grabParent;
	}
}
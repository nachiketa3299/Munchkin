using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	public class PickAction : MonoBehaviour
	{
		private bool isTriggered = false;
		private bool isPicked = false;
		[SerializeField] private Transform _pickPosition;
		private GameObject pickObject;

		public void BeginAction()
		{
			if (!isPicked)
			{
				Debug.Log("Try PICK!!");
				if (isTriggered)
				{
					pickObject.GetComponent<IPickable>().Pick();
					pickObject.transform.position = _pickPosition.position;
					pickObject.transform.parent = _pickPosition;

					Collider[] colliders = pickObject.GetComponents<Collider>();
					foreach (Collider col in colliders)
					{
						col.enabled = false;
					}

					pickObject.GetComponent<Rigidbody>().isKinematic = true;
					pickObject.GetComponent<Rigidbody>().useGravity = false;
					pickObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;

					isPicked = true;
				}
			}
			else
			{
				DropObject();
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			Debug.Log(other.name);
			if (other.tag.Equals("MC_Pickable"))
			{
				isTriggered = true;
				pickObject = other.transform.parent.gameObject;
			}
		}

		public void EndAction()
		{
		}

		private void DropObject()
		{
			Debug.Log("EndPick");
			pickObject.GetComponent<IPickable>().Drop();
			pickObject.transform.position = _pickPosition.position;
			pickObject.transform.parent = null;
			Collider[] colliders = pickObject.GetComponents<Collider>();
			foreach (Collider col in colliders)
			{
				col.enabled = true;
			}

			pickObject.GetComponent<Rigidbody>().isKinematic = false;
			pickObject.GetComponent<Rigidbody>().useGravity = true;
			pickObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
			isPicked = false;
		}
	}
}
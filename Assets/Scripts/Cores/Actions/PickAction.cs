using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MC
{
	public class PickAction : MonoBehaviour
	{
		private bool isTriggered = false;
		[SerializeField] private Transform _pickPosition;
		private GameObject pickObject;

		public void BeginAction()
		{
			Debug.Log("Try PICK!!");
			if (isTriggered)
			{
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
	}
}
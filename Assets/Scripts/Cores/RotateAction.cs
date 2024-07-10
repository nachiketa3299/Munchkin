using UnityEngine;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class RotateAction : MonoBehaviour
	{
		void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public void BeginAction(float desiredDirectionCoeff)
		{
			var currentDirection = transform.forward.x;
			var velocityX = _rigidbody.velocity.x;

			Debug.Log($"Desired: {desiredDirectionCoeff}, VeloX: {velocityX}, Current: {currentDirection}");

		}

		public void EndAction()
		{

		}

		bool _isRotating;

		Rigidbody _rigidbody;
		[SerializeField] float _rotateDuraion;
		[SerializeField] AnimationCurve _rotationCurve;
	}

}
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace MC
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	public class RigidBodyMoveAction : MonoBehaviour, IMoveAction
	{
		#region Unity Messages

		void Awake()
		{
			Assert.IsNotNull(_rigidbody = GetComponent<Rigidbody>());
			Assert.IsNotNull(_moveActionData);
		}

		#endregion // Unity Messages

		void IMoveAction.Move(float directionValue)
		{
			var directionVector = directionValue * Vector3.right;
			var accMag = IsGrounded() ? _moveActionData.AccMagOnGround : _moveActionData.AccMagOnAir;
			var acc = directionVector * accMag;

			_rigidbody.AddForce(acc, ForceMode.Acceleration);
			/// 입력으로부터 가속도를 제대로 계산하고 있는지 확인
			/// Debug.Log($"Acc cal by input: {acc}");
		}

		/// <summary>
		/// 캐릭터가 지면에 발을 붙이고 있는지 아닌지를 판단한다.
		/// </summary>
		private bool IsGrounded()
		{
			// TODO 아직 구현 안됨. CharacterController와 다르게, Rigidbody에는 이게 없음.
			return true;
		}

		Rigidbody _rigidbody;
		[SerializeField] MoveActionData _moveActionData;
	}
}

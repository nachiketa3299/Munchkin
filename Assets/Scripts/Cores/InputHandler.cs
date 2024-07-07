using UnityEngine;

namespace MC
{
	public class InputHandler : MonoBehaviour
	{
		#region Unity Messages

		void Awake()
		{
			_inputActions = new IA_InputActions();

			// Cache & Bind MoveAction

			_moveAction = GetComponent<MoveAction>();

			_inputActions.CharacterActions.Move.performed += (context) =>
			{
				_moveAction.BeginAction(context.ReadValue<float>());
			};

			_inputActions.CharacterActions.Move.canceled += (context) =>
			{
				_moveAction.EndAction();
			};

			// Cache & Bind JumpAction

			_jumpAction = GetComponent<JumpAction>();

			_inputActions.CharacterActions.Jump.performed += (context) =>
			{
				_jumpAction.BeginAction();
			};

			_inputActions.CharacterActions.Jump.canceled += (context) =>
			{
				_jumpAction.EndAction();
			};

		}


		void OnEnable()
		{
			_inputActions.Enable();
		}

		#endregion // Unity Messages

		/// <summary> 
		/// InputActionAsset 에디터 에셋에서 Generate C# Class 옵션으로 생성된 래퍼 클래스입니다. 절대 수정하지 마세요.<br/>
		/// 에디터를 통해 InputActionAsset을 수정한후 Save하면 자동으로 IA_InputAction.cs가 업데이트 됩니다.<br/>
		/// </summary>
		IA_InputActions _inputActions;

		MoveAction _moveAction;
		JumpAction _jumpAction;
	}
}

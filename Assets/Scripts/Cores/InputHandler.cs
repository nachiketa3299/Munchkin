using UnityEngine;

namespace MC
{
	public class InputHandler : MonoBehaviour
	{
		#region Unity Messages

		void Awake()
		{
			_inputActions = new IA_InputActions();

			_moveAction = GetComponent<MoveAction>();

			_inputActions.CharacterActions.Move.performed += (context) =>
			{
				_moveAction.BeginAction(context.ReadValue<float>());
			};

			_inputActions.CharacterActions.Move.canceled += (context) =>
			{
				_moveAction.EndAction();
			};

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

		IA_InputActions _inputActions;
		MoveAction _moveAction;
		JumpAction _jumpAction;
	}
}

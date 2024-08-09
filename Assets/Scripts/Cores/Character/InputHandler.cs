using UnityEngine;

namespace MC
{

public class InputHandler : MonoBehaviour
{
	#region UnityCallbacks

	/// <summary>
	/// 입력 액션과 캐릭터 액션을 바인드. 컴포넌트가 존재하지 않아도 경고 후 실행.
	/// </summary>
	/// NOTE 그냥 이렇게 한땀한땀 적어.
	void Awake()
	{
		_inputActions = new IA_InputActions();

		// Cache & Bind MoveAction

		if (_moveAction = GetComponent<MoveAction>())
		{
			_inputActions.CharacterActions.Move.performed += (context) =>
			{
				_moveAction.BeginAction(context.ReadValue<float>());
			};

			_inputActions.CharacterActions.Move.canceled += (context) =>
			{
				_moveAction.EndAction();
			};
		}

#if UNITY_EDITOR
		else
		{
			Debug.LogWarning($"{typeof(MoveAction).Name} 컴포넌트를 찾을 수 없습니다.");
		}
#endif

		// Cache & Bind JumpAction

		if (_jumpAction = GetComponent<JumpAction>())
		{
			_inputActions.CharacterActions.Jump.performed += (context) =>
			{
				_jumpAction.BeginAction();
			};

			_inputActions.CharacterActions.Jump.canceled += (context) =>
			{
				_jumpAction.EndAction();
			};

		}

#if UNITY_EDITOR
		else
		{
			Debug.LogWarning($"{typeof(JumpAction).Name} 컴포넌트를 찾을 수 없습니다.");
		}
#endif

		// Cache & Bind LookAction

		if (_lookAction = GetComponent<LookAction>())
		{
			_inputActions.CharacterActions.Look.performed += (context) =>
			{
				_lookAction.BeginAction(context.ReadValue<float>());
			};

			_inputActions.CharacterActions.Look.canceled += (context) =>
			{
				_lookAction.EndAction();
			};
		}

#if UNITY_EDITOR
		else
		{
			Debug.LogWarning($"{typeof(LookAction).Name} 컴포넌트를 찾을 수 없습니다.");
		}
#endif

		// Cache & Bind GrabThrowAction

		if (_grabThrowAction = GetComponent<GrabThrowAction>())
		{
			// Grab 상태에 Begin Grab 일 수도, Begin Throw 일 수도 있음.
			_inputActions.CharacterActions.GrabThrow.performed += (context) =>
			{
				var directionValue = _inputActions.CharacterActions.Look.ReadValue<float>();
				_grabThrowAction.BeginAction(directionValue);
			};
		}
#if UNITY_EDITOR
		else
		{
			Debug.LogWarning($"{typeof(GrabThrowAction).Name} 컴포넌트를 찾을 수 없습니다.");
		}
#endif

		// Cache & Bind EggAction

		if (_eggAction = GetComponent<EggAction>())
		{
			_inputActions.CharacterActions.Egg.performed += (context) =>
			{
				_eggAction.BeginAction();
			};

			_inputActions.CharacterActions.Egg.canceled += (context) =>
			{
				_eggAction.EndAction();
			};
		}
		#if UNITY_EDITOR
		else
		{
			Debug.LogWarning($"{typeof(EggAction).Name} 컴포넌트를 찾을 수 없습니다.");
		}
		#endif
	}

	void OnEnable()
	{
		_inputActions.Enable();
	}

	void OnDisable()
	{
		_inputActions.Disable();
	}

#endregion // UnityCallbacks

	/// <summary>
	/// InputActionAsset 에디터 에셋에서 Generate C# Class 옵션으로 생성된 래퍼 클래스입니다. 절대 수정하지 마세요.<br/>
	/// 에디터를 통해 InputActionAsset을 수정한후 Save하면 자동으로 IA_InputAction.cs가 업데이트 됩니다.<br/>
	/// </summary>
	IA_InputActions _inputActions;
	MoveAction _moveAction;
	JumpAction _jumpAction;
	LookAction _lookAction;
	GrabThrowAction _grabThrowAction;
	EggAction _eggAction;
}

}
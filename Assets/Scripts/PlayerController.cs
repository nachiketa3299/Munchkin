using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace Munchkin
{
	public class PlayerController : MonoBehaviour
	{
		#region Unity Messages
		void Awake()
		{
			Assert.IsNotNull(_inputActionAsset);
			_characterActionMap = _inputActionAsset.FindActionMap("CharacterActions");
			_moveAction = _characterActionMap.FindAction("Move");
			_lookAction = _characterActionMap.FindAction("Look");
		}

		void OnEnable()
		{
			_inputActionAsset.Enable();
		}

		void Start()
		{
			var result = GameObject.FindGameObjectWithTag("MC_Character");
			Assert.IsTrue(result);
			_character = result;
		}

		void FixedUpdate()
		{
			_character.GetComponent<IMoveAction>()?.Move(_inputMoveDirectionValue = _moveAction.ReadValue<float>());
			_character.GetComponent<ILookAction>()?.Look(_inputLookDirectionValue = _lookAction.ReadValue<float>());
		}

		void OnDisable()
		{
			_inputActionAsset.Disable();
		}
		#endregion // Unity Messages

		[SerializeField]
		InputActionAsset _inputActionAsset;
		InputActionMap _characterActionMap;
		InputAction _moveAction;
		InputAction _lookAction;
		GameObject _character;
		// 반드시 할 필요는 없지만 혹시 모를 디버그 용도를 위해 일단 입력 값 캐싱
		float _inputMoveDirectionValue;
		float _inputLookDirectionValue;
	}
}

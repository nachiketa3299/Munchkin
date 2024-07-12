using UnityEngine;

namespace MC
{
	/// <summary>
	/// 캐릭터 행동 관련 공통되는 부분의 코드를 줄이기 위해 작성.
	/// </summary>
	public abstract class ActionRoutineBase : MonoBehaviour
	{
		/// <summary>
		/// 현재 실행중인 코루틴이 있다면 멈추고, 없다면 아무 것도 하지 않는다.
		/// </summary>
		protected void TryStopCurrentRoutine()
		{
			if (_currentRoutine == null)
			{
				return;
			}

			StopCoroutine(_currentRoutine);
			_currentRoutine = null;
		}

		/// <summary>
		/// 현재 실행중인 코루틴
		/// </summary>
		protected Coroutine _currentRoutine = null;
	}
}

// NOTE
// BeginAction, BeginAction(float), EndAction() 등을
// Interface 로 구현하면 어떨까 했지만, 불필요한 복잡성을 추가하는 것 같아서 하지 않음.
// NOTE 파일을 추후에 분리해야 할 수도 있음

namespace Munchkin
{
	public interface IMoveAction
	{
		/// <summary>
		/// 캐릭터를 전달된 방향으로 이동시킨다.
		/// </summary>
		/// <param name="directionValue">
		/// 	-1.0f 에서 1.0f 사이의 값.
		/// 	-1.0f는 왼쪽(-1.0f, 0.0f, 0.0f)을 의미하고, 1.0f는 오른쪽(1.0f, 0.0f, 0.0f)를 의미한다.
		/// </param>
		void Move(float directionValue);
	}

	public interface ILookAction
	{
		/// <summary>
		/// 카메라의 중심을 전달된 방향으로 이동시킨다.
		/// </summary>
		/// <param name="directionValue">
		/// -1.0f 에서 1.0f 사이의 값.
		/// -1.0f는 왼쪽(-1.0f, 0.0f, 0.0f)을 의미하고, 1.0f는 오른쪽(1.0f, 0.0f, 0.0f)를 의미한다.
		/// </param>
		void Look(float directionValue);
	}
}
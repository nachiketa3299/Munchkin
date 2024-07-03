// NOTE 파일을 추후에 분리해야 할 수도 있음

namespace MC
{
	public interface IMoveAction
	{
		/// <summary> 캐릭터를 <paramref name="directionValue"/> 방향으로 이동시킨다. </summary>
		void Move(float directionValue);
	}

	public interface ILookAction
	{
		/// <summary> 카메라의 중심을 <paramref name="directionValue"/> 방향으로 이동시킨다. </summary>
		/// <remarks> -1.0f 는 왼쪽(-1.0f, 0.0f, 0.0f), 1.0f는 오른쪽(1.0f, 0.0f, 0.0f)이다. </remarks>
		void Look(float directionValue);
	}
	public interface IEggAction
	{
		void LayEgg();
	}
}
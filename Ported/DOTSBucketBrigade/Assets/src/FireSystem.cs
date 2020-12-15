using Unity.Entities;
using Unity.Mathematics;

public struct BoardElement : IBufferElementData
{
	// represents the heat of the cell element.
	// 0 -> no heat, 1 -> on fire
	public float Value;

	public static implicit operator float(BoardElement e)
	{
		return e.Value;
	}

	public static implicit operator BoardElement (float e)
	{
		return new BoardElement { Value = e };
	}
}

public class FireSystem : SystemBase
{
	Entity m_BoardEntity;
	
	protected override void OnCreate()
	{
		m_BoardEntity = EntityManager.CreateEntity();
		DynamicBuffer<BoardElement> boardCells = EntityManager.AddBuffer<BoardElement>(m_BoardEntity);
		boardCells.EnsureCapacity(FireSimConfig.xDim * FireSimConfig.yDim);

		for (int i=0; i<boardCells.Length; ++i)
		{
			boardCells[i] = 0.0f;
		}
	}
	
	unsafe protected override void OnUpdate()
	{
		var xDim = FireSimConfig.xDim;
		var yDim = FireSimConfig.yDim;

		int2* neighborOffsets = stackalloc []
		{
			new int2(+0, -1),
			new int2(+1, -1),
			new int2(+1, +0),
			new int2(+1, +1),
			new int2(+0, +1),
			new int2(-1, +1),
			new int2(-1, +0),
			new int2(-1, -1)
		};
		
		Entities.ForEach((ref DynamicBuffer<BoardElement> board) =>
		{
			for (int i=0; i<board.Length; ++i)
			{
				
			}
		}).Schedule();
	}
}

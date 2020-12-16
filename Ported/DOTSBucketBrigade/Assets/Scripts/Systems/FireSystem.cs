using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;

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

public struct BoardDebugElement : IBufferElementData
{
	public uint Value;

	public static implicit operator uint(BoardDebugElement e)
	{
		return e.Value;
	}

	public static implicit operator BoardDebugElement (uint e)
	{
		return new BoardDebugElement { Value = e };
	}
}

public class FireSystem : SystemBase
{
	Entity m_BoardEntity;
	NativeArray<int2> m_NeighborOffsets;

	protected override void OnCreate()
	{
		System.Random fireRandom = new System.Random(1234);
		
		m_BoardEntity = EntityManager.CreateEntity();
#if BB_DEBUG_FLAGS
		DynamicBuffer<BoardDebugElement> boardDebugFlags = EntityManager.AddBuffer<BoardDebugElement>(m_BoardEntity);
		boardDebugFlags.ResizeUninitialized(FireSimConfig.xDim * FireSimConfig.yDim);
		for (int i=0; i<boardDebugFlags.Length; ++i)
			boardDebugFlags[i] = 0U;
#endif
		DynamicBuffer<BoardElement> boardCells = EntityManager.AddBuffer<BoardElement>(m_BoardEntity);
		boardCells.ResizeUninitialized(FireSimConfig.xDim * FireSimConfig.yDim);
		
		for (int i = 0; i < boardCells.Length; ++i)
		{
			boardCells[i] = 0.0f;
		}

		//start the fires.
		for (int i = 0; i < FireSimConfig.numFireStarters; ++i)
		{
			boardCells[fireRandom.Next(0, boardCells.Length)] = 1.0f;
		}

		//set the water sources within the grid - unneeded once rivers exist
		/*
		for (int i = 0; i < FireSimConfig.numWaterSources; ++i)
		{
			boardCells[fireRandom.Next(0, boardCells.Length)] = -0.5f;
		}
		*/

		m_NeighborOffsets = new NativeArray<int2>(8, Allocator.Persistent);
		NativeArray<int2>.Copy(new [] {new int2(+0, -1),
			new int2(+1, -1),
			new int2(+1, +0),
			new int2(+1, +1),
			new int2(+0, +1),
			new int2(-1, +1),
			new int2(-1, +0),
			new int2(-1, -1)}, m_NeighborOffsets);
	}

	protected override void OnDestroy()
	{
		m_NeighborOffsets.Dispose();
	}

	protected override void OnUpdate()
	{
		var xDim = FireSimConfig.xDim;
		var yDim = FireSimConfig.yDim;

		var neighborOffsets = m_NeighborOffsets;
		var newHeat = new NativeArray<float>(xDim*yDim, Allocator.TempJob);
		var heatTransferRate = FireSimConfig.heatTransferRate;

		var currentDeltaTime = Time.DeltaTime;

		float4 waterColor = new float4(FireSimConfig.color_watersource.r, FireSimConfig.color_watersource.g, FireSimConfig.color_watersource.b, FireSimConfig.color_watersource.a);
		float4 groundColor = new float4(FireSimConfig.color_ground.r, FireSimConfig.color_ground.g, FireSimConfig.color_ground.b, FireSimConfig.color_ground.a);
		float4 fireLowColor = new float4(FireSimConfig.color_fire_low.r, FireSimConfig.color_fire_low.g, FireSimConfig.color_fire_low.b, FireSimConfig.color_fire_low.a);
		float4 fireHighColor = new float4(FireSimConfig.color_fire_high.r, FireSimConfig.color_fire_high.g, FireSimConfig.color_fire_high.b, FireSimConfig.color_fire_high.a);


#if BB_DEBUG_FLAGS
		var lookup = GetBufferFromEntity<BoardDebugElement>();
		DynamicBuffer<BoardDebugElement> boardDebugElementBuffer = lookup[m_BoardEntity];
		NativeArray<uint> debugFlags = boardDebugElementBuffer.Reinterpret<uint>().AsNativeArray(); // jiv fixme should probably use a SingleEntity, but really this should be Blob data
#endif

		Entities.ForEach((in DynamicBuffer<BoardElement> board) =>
		{
			for (int i=0; i<board.Length; ++i)
			{
				/* - not counting for water now
				if (board[i] < -0.1f) // if the cell is water, don't burn it
				{
					newHeat[i] = board[i]; // was going to have the water slowly dry out + 0.00001f;
				}
				else
				{
				*/
					//bool waterAdjacent = false;
					float heatValue = 0;
					int2 coord = new int2(i % xDim, i / xDim);
					for (int j = 0; j < 8; j++)
					{
						var neighbor = neighborOffsets[j];
						int2 neighborCoord = coord + neighbor;
						if (math.any(neighborCoord >= new int2(xDim, yDim)) ||
							math.any(neighborCoord < int2.zero))
						{
							continue;
						}

						float desiredHeatDelta = board[neighborCoord.y * xDim + neighborCoord.x];
						//if (desiredHeatDelta > -0.01f)
						//{
							heatValue += desiredHeatDelta;
						//}
						//else
						//{
						//	waterAdjacent = true;
						//}
					}

					//if (!waterAdjacent) 
					newHeat[i] = Math.Min(1.0f, board[i] + (heatTransferRate * heatValue * currentDeltaTime));
				//}
			}
		}).Schedule();

		var flashPoint = FireSimConfig.flashPoint;
		var fireThreshold = FireSimConfig.fireThreshold;

		Entities
			.WithReadOnly(newHeat)
			.ForEach((ref Translation translation, ref URPMaterialPropertyBaseColor fireColor, in FireCell fireCell) =>
			{
				var index = fireCell.coord.y * xDim + fireCell.coord.x;
				float3 newTranslation = translation.Value;
				newTranslation.x = fireCell.coord.x; 
				newTranslation.y = newHeat[index];
				newTranslation.z = fireCell.coord.y;
				translation.Value = newTranslation;

				if (newHeat[index] > flashPoint) { fireColor.Value = fireHighColor; }
				else if (newHeat[index] > fireThreshold) { fireColor.Value = fireLowColor; }
				else if (newHeat[index] < -0.1f) { fireColor.Value = waterColor; }
				else fireColor.Value = groundColor;

#if BB_DEBUG_FLAGS
				if (debugFlags[index] != 0)
				{
					fireColor.Value = new float4(1,0,1,1);
				}
#endif
			}
		).Schedule();

		Entities
			.WithDisposeOnCompletion(newHeat)
			.ForEach((ref DynamicBuffer<BoardElement> board) =>
		{
			for (int i = 0; i < board.Length; ++i)
			{
				board[i] = newHeat[i];
			}
		}).Schedule();

#if BB_DEBUG_FLAGS
		MemsetNativeArray<uint> memsetNativeArray = new MemsetNativeArray<uint>
		{
			Source = debugFlags,
			Value = 0
		};
		
		Dependency = memsetNativeArray.Schedule(debugFlags.Length, 64, Dependency);
#endif
	}
}

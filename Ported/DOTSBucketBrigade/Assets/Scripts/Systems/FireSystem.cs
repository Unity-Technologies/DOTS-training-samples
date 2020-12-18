using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Jobs;
using Random = Unity.Mathematics.Random;

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

public struct DouseElement : IBufferElementData
{
	public int Value;

	public static implicit operator int(DouseElement e)
	{
		return e.Value;
	}

	public static implicit operator DouseElement (int e)
	{
		return new DouseElement { Value = e };
	}
}

#if BB_DEBUG_FLAGS
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
#endif

struct HeatJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<int2> neighborOffsets;

    [ReadOnly]
    public float currentDeltaTime;

    [ReadOnly]
    public int2 dims;

    [ReadOnly]
    public float heatTransferRate;

    [ReadOnly]
    public uint randomSeed;

    [ReadOnly]
    public DynamicBuffer<BoardElement> board;

    public NativeArray<float> newHeat;

    public void Execute(int i)
    {
        // do the thing
        float heatValue = 0;
        int2 coord = new int2(i % dims.x, i / dims.x);

        for (int j = 0; j < 8; j++)
        {
            var neighbor = neighborOffsets[j];
            int2 neighborCoord = coord + neighbor;
            if (math.any(neighborCoord >= new int2(dims.x, dims.y)) ||
                math.any(neighborCoord < int2.zero))
            {
                continue;
            }

            float desiredHeatDelta = board[neighborCoord.y * dims.x + neighborCoord.x];
            heatValue += desiredHeatDelta;
        }

        newHeat[i] = Math.Min(1.0f, board[i] + (heatTransferRate * heatValue * currentDeltaTime));

        // introduce a tiny bit of randomness for flames
        if (newHeat[i] > 0.8f)
        {
            Random fireRandom = new Random(randomSeed + (uint)i);
            newHeat[i] -= fireRandom.NextFloat(0.0f, 0.15f);
        }
    }
}

public class FireSystem : SystemBase
{
	Entity m_BoardEntity;
	NativeArray<int2> m_NeighborOffsets;

	protected override void OnCreate()
	{
        Random fireRandom = new Random((uint)System.Environment.TickCount);


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
			boardCells[fireRandom.NextInt(0, boardCells.Length)] = 1.0f;
		}

        // jiv fixme: should be hashset
		DynamicBuffer<DouseElement> douseCells = EntityManager.AddBuffer<DouseElement>(m_BoardEntity);

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
        Random fireRandom = new Random((uint)System.Environment.TickCount);

        var xDim = FireSimConfig.xDim;
		var yDim = FireSimConfig.yDim;

		var neighborOffsets = m_NeighborOffsets;
		var newHeat = new NativeArray<float>(xDim*yDim, Allocator.TempJob);
		var heatTransferRate = FireSimConfig.heatTransferRate;

		var currentDeltaTime = Time.DeltaTime;

        float4 waterColor = FireSimConfig.color_watersource;
        float4 groundColor = FireSimConfig.color_ground;
        float4 fireLowColor = FireSimConfig.color_fire_low;
        float4 fireHighColor = FireSimConfig.color_fire_high;
        float4 fireHighColor1 = FireSimConfig.color_fire_high1;
        float4 fireHighColor2 = FireSimConfig.color_fire_high2;
        float4 fireHighColor3 = FireSimConfig.color_fire_high3;
        float4 fireHighColor4 = FireSimConfig.color_fire_high4;

#if BB_DEBUG_FLAGS
		var lookup = GetBufferFromEntity<BoardDebugElement>();
		DynamicBuffer<BoardDebugElement> boardDebugElementBuffer = lookup[m_BoardEntity];
		NativeArray<uint> debugFlags = boardDebugElementBuffer.Reinterpret<uint>().AsNativeArray(); // jiv fixme should probably use a SingleEntity, but really this should be Blob data
#endif
        DynamicBuffer<BoardElement> board = GetBuffer<BoardElement>(GetSingletonEntity<BoardElement>());
        var heatJob = new HeatJob
        {
            randomSeed = (uint)System.Environment.TickCount,
            board = board,
            currentDeltaTime = Time.DeltaTime,
            dims = new int2(xDim, yDim),
            heatTransferRate = heatTransferRate,
            neighborOffsets = m_NeighborOffsets,
            newHeat = newHeat
        };

        Dependency = heatJob.Schedule(board.Length, 2048, Dependency);

        // douse fire
        Entities
            .ForEach((ref DynamicBuffer<DouseElement> douseElement) =>
            {
                for (int i=0; i<douseElement.Length; ++i)
                {
                    newHeat[douseElement[i]] = 0.0f;
                }
                douseElement.Clear();
            }).Schedule();

        var flashPoint = FireSimConfig.flashPoint;
		var fireThreshold = FireSimConfig.fireThreshold;

		Entities
			.WithReadOnly(newHeat)
			.ForEach((ref LocalToWorld localToWorld, ref URPMaterialPropertyBaseColor fireColor, in FireCell fireCell) =>
			{
                float4 colorLerp(float4 from, float4 to, float t)
                {
                    return from + (to - from) * t;
                }

				var index = fireCell.coord.y * xDim + fireCell.coord.x;
				float4 newTranslation = localToWorld.Value.c3;
				newTranslation.x = fireCell.coord.x;
				newTranslation.y = newHeat[index];
				newTranslation.z = fireCell.coord.y;
                newTranslation.w = localToWorld.Value.c3.w;
				localToWorld.Value.c3 = newTranslation;

                fireColor.Value = colorLerp(groundColor, fireHighColor4, newHeat[index]);
                 var h = newHeat[index];
                 if (h < 0) fireColor.Value = waterColor;
                 else if (h < fireThreshold) fireColor.Value = colorLerp(groundColor, fireLowColor, h / fireThreshold);
                 else if (h < 0.5f) fireColor.Value = colorLerp(fireLowColor, fireHighColor, (h - fireThreshold) / 0.3f);
                 else if (h < 0.6f) fireColor.Value = colorLerp(fireHighColor, fireHighColor1, (h - 0.5f) / 0.1f);
                 else if (h < 0.7f) fireColor.Value = colorLerp(fireHighColor1, fireHighColor2, (h - 0.6f) / 0.1f);
                 else if (h < 0.8f) fireColor.Value = colorLerp(fireHighColor2, fireHighColor3, (h - 0.7f) / 0.1f);
                 else if (h < 0.9f) fireColor.Value = colorLerp(fireHighColor3, fireHighColor4, (h - 0.8f) / 0.1f);
                 else fireColor.Value = fireHighColor4;

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
			.ForEach((ref DynamicBuffer<BoardElement> boardBuffer) =>
		{
			for (int i = 0; i < boardBuffer.Length; ++i)
			{
				boardBuffer[i] = newHeat[i];
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

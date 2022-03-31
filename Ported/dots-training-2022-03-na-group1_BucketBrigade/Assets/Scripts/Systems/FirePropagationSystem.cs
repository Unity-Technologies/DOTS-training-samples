using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

public partial class FirePropagationSystem : SystemBase
{
    public const float HEAT_THRESHOLD = 0.2f;
    
    int firePropagationRadius = 2;

    NativeArray<int2> checkAdjacents;
    NativeArray<int> adjacentIndicesArray;
    protected override void OnCreate()
    {
        GridUtility.CreateAdjacentTileArray(ref checkAdjacents,firePropagationRadius);
        
        adjacentIndicesArray = new NativeArray<int>(checkAdjacents.Length,Allocator.Persistent);
    }
    protected override void OnDestroy()
    {
        checkAdjacents.Dispose();
        adjacentIndicesArray.Dispose();
    }

    protected override void OnUpdate()
    {
        var localCheckAdjacents = checkAdjacents;
        var adjacentIndices = adjacentIndicesArray;

        var heatmapData = GetSingleton<HeatMapData>();
        var heatmapBuffer = GetHeatmapBuffer();
        
        float deltaTime = Time.DeltaTime * heatmapData.heatPropagationSpeed;

        var job = new HeatJob()
        {
            width = heatmapData.mapSideLength,
            adjacentOffsets = localCheckAdjacents,
            deltaTime = deltaTime,
            heatmapBuffer = heatmapBuffer
        };

        var handle = job.Schedule(heatmapBuffer.Length, 32);
        handle.Complete();
        
#if false
        Job.WithCode(() => {

            for (var i = 0; i < heatmapBuffer.Length; i++)
            {
                if (heatmapBuffer[i] >= HEAT_THRESHOLD)
                {
                    heatmapBuffer[i] += deltaTime;
                    HeatAdjacents(ref heatmapBuffer, localCheckAdjacents, adjacentIndices, i, heatmapData.mapSideLength, deltaTime);
                }

                if (heatmapBuffer[i] >= 1f)
                    heatmapBuffer[i] = 1f;
            }

        }).Run();
#endif
    }

    static void HeatAdjacents(
        ref DynamicBuffer<HeatMapTemperature> buffer, 
        NativeArray<int2> adjacentOffsets, 
        NativeArray<int> adjacentIndices,
        int tileIndex,  int width, float deltaTime)
    {
#if true
        for (int iCheck = 0; iCheck < adjacentOffsets.Length; iCheck++)
        {
            int2 tileCoord = GridUtility.GetTileCoordinate(tileIndex, width);
            int x = tileCoord.x + adjacentOffsets[iCheck].x;
            int z = tileCoord.y + adjacentOffsets[iCheck].y;

            bool inBounds = (x >= 0 && x <= width-1 && z >= 0 && z <= width-1);

            if (inBounds)
            {
                int adjacentIndex = GridUtility.GetTileIndex(x, z, width);
                
                if(buffer[adjacentIndex] < HEAT_THRESHOLD)
                    buffer[adjacentIndex] += deltaTime;
            }
        }
#else
        var job = new HeatAdjacentsJob()
        {
            width = width,
            tileIndex = tileIndex,
            adjacentOffsets = adjacentOffsets,
            adjacentIndexArray = adjacentIndices
        };

        var handle = job.Schedule(adjacentOffsets.Length, 1);
        handle.Complete();

        for (int iCheck = 0; iCheck < adjacentIndices.Length; iCheck++)
        {
            int adjacentIndex = adjacentIndices[iCheck];
            if( adjacentIndex >= 0 && buffer[adjacentIndex] < HEAT_THRESHOLD)
                buffer[adjacentIndex] += deltaTime;
        }
#endif
    }
    
    [BurstCompile]
    struct HeatJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<int2> adjacentOffsets;
        
        [NativeDisableParallelForRestriction]
        public DynamicBuffer<HeatMapTemperature> heatmapBuffer;
        
        public float deltaTime;
        public int width;
        public void Execute(int tileIndex)
        {
            if (heatmapBuffer[tileIndex] >= HEAT_THRESHOLD)
            {
                heatmapBuffer[tileIndex] += deltaTime;

                for (int iAdjacent = 0; iAdjacent < adjacentOffsets.Length; iAdjacent++)
                {
                    int2 tileCoord = GridUtility.GetTileCoordinate(tileIndex, width);
                    int x = tileCoord.x + adjacentOffsets[iAdjacent].x;
                    int z = tileCoord.y + adjacentOffsets[iAdjacent].y;

                    bool inBounds = (x >= 0 && x <= width - 1 && z >= 0 && z <= width - 1);

                    if (inBounds)
                    {
                        int adjacentIndex = GridUtility.GetTileIndex(x, z, width);

                        if (heatmapBuffer[adjacentIndex] < HEAT_THRESHOLD)
                            heatmapBuffer[adjacentIndex] += deltaTime;
                    }
                }
            }

            if (heatmapBuffer[tileIndex] >= 1f)
                heatmapBuffer[tileIndex] = 1f;
        }
    }

    DynamicBuffer<HeatMapTemperature> GetHeatmapBuffer() 
    {
        var heatmap = GetSingletonEntity<HeatMapTemperature>();
        return EntityManager.GetBuffer<HeatMapTemperature>(heatmap);
    }
    
    DynamicBuffer<HeatMapSplash> GetSplashmapBuffer() 
    {
        var splashmap = GetSingletonEntity<HeatMapSplash>();
        return EntityManager.GetBuffer<HeatMapSplash>(splashmap);
    }

}

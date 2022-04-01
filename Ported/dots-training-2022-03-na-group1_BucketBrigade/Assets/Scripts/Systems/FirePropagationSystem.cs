using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

public partial class FirePropagationSystem : SystemBase
{
    public const float HEAT_THRESHOLD = 0.2f;
    public const int firePropagationRadius = 2;

    private NativeArray<float> heatIncrementBuffer;
    private int bufferLength;
    
    protected override void OnCreate()
    {
    }
    protected override void OnDestroy()
    {
        heatIncrementBuffer.Dispose();
    }

    protected override void OnUpdate()
    {
        var heatmapData = GetSingleton<HeatMapData>();
        var heatmapBuffer = BucketBrigadeUtility.GetHeatmapBuffer(this);

        if (bufferLength == 0)
        {
            bufferLength = heatmapData.mapSideLength * heatmapData.mapSideLength;
            heatIncrementBuffer = new NativeArray<float>( bufferLength, Allocator.Persistent);
        }

        float deltaHeat = Time.DeltaTime * heatmapData.heatPropagationSpeed;
        
        var jobFirst = new HeatIncrementJob()
        {
            width = heatmapData.mapSideLength,
            deltaHeat = deltaHeat,
            heatmapBuffer = heatmapBuffer,
            heatIncrementArray = heatIncrementBuffer,
        };
        
        var jobSecond = new HeatUpdateJob()
        {
            heatmapBuffer = heatmapBuffer,
            heatIncrementArray = heatIncrementBuffer
        };

        var handle = jobFirst.Schedule(heatmapBuffer.Length, 32);
        handle.Complete();

        var secondHandle = jobSecond.Schedule(heatmapBuffer.Length, 32, handle);
        secondHandle.Complete();
    }
    
    [BurstCompile]
    struct HeatIncrementJob : IJobParallelFor
    {
        [ReadOnly] public DynamicBuffer<HeatMapTemperature> heatmapBuffer;
        
        public NativeArray<float> heatIncrementArray;
        
        public float deltaHeat;
        public int width;
        
        public void Execute(int tileIndex)
        {
            heatIncrementArray[tileIndex] = 0f;
            int2 tileCoord = GridUtility.GetTileCoordinate(tileIndex, width);

            for (int rowIndex = -firePropagationRadius; rowIndex <= firePropagationRadius; rowIndex++)
            {
                int currentRow = tileCoord.x - rowIndex;
                if (currentRow >= 0 && currentRow < width)
                {
                    for (int columnIndex = -firePropagationRadius; columnIndex <= firePropagationRadius; columnIndex++)
                    {
                        int currentColumn = tileCoord.y + columnIndex;
                        if (currentColumn >= 0 && currentColumn < width)
                        {
                            float _neighbourHeat = heatmapBuffer[(currentRow * width) + currentColumn];
                            if (_neighbourHeat >= HEAT_THRESHOLD)
                            {
                                heatIncrementArray[tileIndex] += deltaHeat;
                            }
                        }
                    }
                }
            }

            /*
            if (heatmapBuffer[tileIndex] >= HEAT_THRESHOLD)
            {
                heatIncrementArray[tileIndex] += deltaHeat;

                //heat adjacents
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
                        {
                            heatIncrementArray[adjacentIndex] += deltaHeat;
                        }
                            
                    }
                }
            }*/
        }
    }

    [BurstCompile]
    struct HeatUpdateJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> heatIncrementArray;
        
        [NativeDisableParallelForRestriction]
        public DynamicBuffer<HeatMapTemperature> heatmapBuffer;

        public void Execute(int tileIndex)
        {
            heatmapBuffer[tileIndex] += heatIncrementArray[tileIndex];
            
            if (heatmapBuffer[tileIndex] >= 1f)
                heatmapBuffer[tileIndex] = 1f;
        }   
    }
}

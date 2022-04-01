using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;

public partial class FirePropagationSystem : SystemBase
{
    public const float HEAT_THRESHOLD = 0.2f;
    public const int firePropagationRadius = 2;

    protected override void OnUpdate()
    {
        var heatmapData = GetSingleton<HeatMapData>();
        var heatmapBuffer = BucketBrigadeUtility.GetHeatmapBuffer(this);

        float deltaHeat = Time.DeltaTime * heatmapData.heatPropagationSpeed;
        
        var jobFirst = new HeatJob()
        {
            width = heatmapData.mapSideLength,
            deltaHeat = deltaHeat,
            heatmapBuffer = heatmapBuffer,
        };
        
        var handle = jobFirst.Schedule(heatmapBuffer.Length, 32);
        handle.Complete();
        
    }
    
    [BurstCompile]
    struct HeatJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction] 
        public DynamicBuffer<HeatMapTemperature> heatmapBuffer;
        
        public float deltaHeat;
        public int width;
        
        public void Execute(int tileIndex)
        {
            float heatIncrement = 0f;
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
                                heatIncrement += deltaHeat;
                        }
                    }
                }
            }

            heatmapBuffer[tileIndex] += heatIncrement;
            if (heatmapBuffer[tileIndex] >= 1f)
                heatmapBuffer[tileIndex] = 1f;

            /*
             Old implementation, wrong approach. Instead of MainTile adding heat to neighbors, 
             the solution is to add heat to the MainTile by looking if neighbors are on fire.
             MainTile -> Neighbors = wrong
             Neighbors -> MainTile = correct
             
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
}

using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public partial class FirePropagationSystem : SystemBase
{
    public const float HEAT_THRESHOLD = 0.2f;
    
    int firePropagationRadius = 2;

    NativeArray<int2> checkAdjacents;

    protected override void OnCreate()
    {
        GridUtil.CreateAdjacentTileArray(ref checkAdjacents,firePropagationRadius);
        
        
    }
    protected override void OnDestroy()
    {
        checkAdjacents.Dispose();
    }

    protected override void OnUpdate()
    {
        var localCheckAdjacents = checkAdjacents;

        var heatmapData = GetSingleton<HeatMapData>();
        var heatmapBuffer = GetHeatmapBuffer();
        
        float deltaTime = Time.DeltaTime * heatmapData.heatPropagationSpeed;

        Job.WithCode(() => {

            for (var i = 0; i < heatmapBuffer.Length; i++)
            {
                if (heatmapBuffer[i] >= HEAT_THRESHOLD)
                {
                    heatmapBuffer[i] += deltaTime;
                    HeatAdjacents(ref heatmapBuffer, localCheckAdjacents, i, heatmapData.mapSideLength, deltaTime);
                }

                if (heatmapBuffer[i] >= 1f)
                    heatmapBuffer[i] = 1f;
            }

        }).Run();
    }

    static void HeatAdjacents(
        ref DynamicBuffer<HeatMapTemperature> buffer, 
        NativeArray<int2> adjacentOffsets, 
        int tileIndex,  int width, float deltaTime)
    {
        for (int iCheck = 0; iCheck < adjacentOffsets.Length; iCheck++)
        {
            int2 tileCoord = GridUtil.GetTileCoordinate(tileIndex, width);
            int x = tileCoord.x + adjacentOffsets[iCheck].x;
            int z = tileCoord.y + adjacentOffsets[iCheck].y;

            bool inBounds = (x >= 0 && x <= width-1 && z >= 0 && z <= width-1);

            if (inBounds)
            {
                int adjacentIndex = GridUtil.GetTileIndex(x, z, width);
                
                if(buffer[adjacentIndex] < HEAT_THRESHOLD)
                    buffer[adjacentIndex] += deltaTime;
            }
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

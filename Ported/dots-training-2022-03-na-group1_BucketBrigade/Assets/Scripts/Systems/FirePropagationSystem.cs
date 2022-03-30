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
        CreateAdjacentOffsetArray(ref checkAdjacents,firePropagationRadius);
    }
    protected override void OnDestroy()
    {
        checkAdjacents.Dispose();
    }

    public static void CreateAdjacentOffsetArray(ref NativeArray<int2> array, int radius)
    {
        //radius 1 -> 8  : 3*3-1
        //radius 2 -> 24 : 5*5-1 : (r+r+1)^2-1
        
        int diameter = (2 * radius + 1);
        int arrayLength = diameter * diameter - 1;
        
        array = new NativeArray<int2>(arrayLength, Allocator.Persistent);

        int i = 0;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                array[i] = new int2(x,y);
                i++;
            }
        }
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

        var splashmapBuffer = GetSplashmapBuffer();
        
        Job.WithCode(() => {

            for (var i = 0; i < splashmapBuffer.Length; i++)
            {
                if (splashmapBuffer[i] < 0)//-1
                    continue;
                
                SuppressAdjacents(ref heatmapBuffer, localCheckAdjacents,splashmapBuffer[i],heatmapData.mapSideLength);
            }

        }).Run();
    }

    static void HeatAdjacents(ref DynamicBuffer<HeatMapTemperature> buffer, NativeArray<int2> checkAdjacents, int tileIndex, int width, float deltaTime)
    {
        for (int iCheck = 0; iCheck < checkAdjacents.Length; iCheck++)
        {
            int2 tileCoord = GetTileCoordinate(tileIndex, width);
            int x = tileCoord.x + checkAdjacents[iCheck].x;
            int z = tileCoord.y + checkAdjacents[iCheck].y;

            bool inBounds = (x >= 0 && x <= width-1 && z >= 0 && z <= width-1);

            if (inBounds)
            {
                int adjacentIndex = GetTileIndex(x, z, width);
                
                if(buffer[adjacentIndex] < HEAT_THRESHOLD)
                    buffer[adjacentIndex] += deltaTime;
            }
        }
    }
    
    static void SuppressAdjacents(
        ref DynamicBuffer<HeatMapTemperature> buffer, 
        NativeArray<int2> adjacentOffsets, 
        int tileIndex, int width)
    {
        for (int iCheck = 0; iCheck < adjacentOffsets.Length; iCheck++)
        {
            int2 tileCoord = GetTileCoordinate(tileIndex, width);
            int x = tileCoord.x + adjacentOffsets[iCheck].x;
            int z = tileCoord.y + adjacentOffsets[iCheck].y;

            bool inBounds = (x >= 0 && x <= width-1 && z >= 0 && z <= width-1);

            if (inBounds)
            {
                int adjacentIndex = GetTileIndex(x, z, width);
                buffer[adjacentIndex] = 0f;
            }
        }
    }

   static int GetTileIndex(int x, int z, int width)
    {
        return (z * width) + x;
    }
    
    static int2 GetTileCoordinate(int index , int width)
    {
        int x = index / width;
        int z = index % width;
        return new int2(x, z);
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

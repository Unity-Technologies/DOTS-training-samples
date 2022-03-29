using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Collections;

public partial class FirePropagationSystem : SystemBase
{

    private NativeArray<int2> checkAdjacents;

    protected override void OnCreate()
    {
        checkAdjacents = new NativeArray<int2>(8, Allocator.Persistent);

        checkAdjacents[0] = new int2(-1, -1);
        checkAdjacents[1] = new int2(0, -1);
        checkAdjacents[2] = new int2(1, -1);
        checkAdjacents[3] = new int2(-1, 0);
        checkAdjacents[4] = new int2(1, 0);
        checkAdjacents[5] = new int2(-1, 1);
        checkAdjacents[6] = new int2(0, 1);
        checkAdjacents[7] = new int2(1, 1);
    }

    protected override void OnUpdate()
    {
        var heatmapData = GetSingleton<HeatMapData>(); 
        
        var heatmap = GetSingletonEntity<HeatMapTemperature>();
        DynamicBuffer<HeatMapTemperature> heatmapBuffer = EntityManager.GetBuffer<HeatMapTemperature>(heatmap);
        
        float deltaTime = Time.DeltaTime * heatmapData.heatSpeed;
        
        var buffer = heatmapBuffer.Reinterpret<float>();

        var localCheckAdjacents = checkAdjacents;

        Job.WithCode(() => {

            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] >= 0.2f)
                {
                    buffer[i] += deltaTime;
                    HeatAdjacents(ref heatmapBuffer, localCheckAdjacents, i, heatmapData.width, deltaTime);
                }

                if (buffer[i] >= 1f)
                    buffer[i] = 1f;
            }

        }).Run();

        Entities.WithAll<FireIndex>()
             .ForEach( (ref FireIndex fireIndex, ref URPMaterialPropertyBaseColor colorComponent, ref Translation translation) =>
             {
                 float intensity = heatmapBuffer[fireIndex.index];
                 colorComponent.Value = math.lerp(heatmapData.startColor, heatmapData.finalColor, intensity);

                 Translation currentTranslation = translation;
                 currentTranslation.Value.y = math.lerp(0f, heatmapData.maxTileHeight, intensity);
                 translation = currentTranslation;
             })
             .Schedule();
    }

    static void HeatAdjacents(ref DynamicBuffer<HeatMapTemperature> buffer, NativeArray<int2> checkAdjacents, int tileIndex, int width, float deltaTime)
    {
        //check out of bounds
        //(x-1, z-1), (x, z-1), (x+1, z-1) 
        //(x-1, z  ), *(x, z)*, (x+1, z  ) 
        //(x-1, z+1), (x, z+1), (x+1, z+1) 

        for (int iCheck = 0; iCheck < checkAdjacents.Length; iCheck++)
        {
            int2 tileCoord = GetTileCoordinate(tileIndex, width);
            int x = tileCoord.x + checkAdjacents[iCheck].x;
            int z = tileCoord.y + checkAdjacents[iCheck].y;

            bool inBounds = (x >= 0 && x <= width-1 && z >= 0 && z <= width-1);

            int adjacentIndex = GetTileIndex(x, z, width);

            if (inBounds && buffer[adjacentIndex] < 0.2f)
            {
                //heat
                buffer[adjacentIndex] += deltaTime;
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
}

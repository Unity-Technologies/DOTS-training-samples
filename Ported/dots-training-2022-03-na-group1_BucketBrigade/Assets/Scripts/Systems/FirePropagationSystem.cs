using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;

public partial class FirePropagationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var heatmapData = GetSingleton<HeatMapData>(); 
        
        var heatmap = GetSingletonEntity<HeatMapTemperature>();
        DynamicBuffer<HeatMapTemperature> heatmapBuffer = EntityManager.GetBuffer<HeatMapTemperature>(heatmap);
        //
        // var intDynamicBuffer = heatmapBuffer.Reinterpret<float>();
        // for (int i = 0; i < heatmapBuffer.Length; i++)
        // {
        //     // ...
        //     intDynamicBuffer[i] = 0.5f;
        // }
        
         float deltaTime = Time.DeltaTime * heatmapData.heatSpeed;
         
         Entities
             .ForEach( (ref DynamicBuffer<HeatMapTemperature> heatmapBuffer) =>
             {
                 var buffer = heatmapBuffer.Reinterpret<float>();
                 for (var i = 0; i < buffer.Length; i++)
                 {
                     if (buffer[i] >= 0.2f)
                     {
                         buffer[i] += deltaTime;
                         HeatAdjacents(ref heatmapBuffer, i, heatmapData.width, deltaTime);
                     }

                     if (buffer[i] >= 1f)
                         buffer[i] = 1f;
                 }
             })
             .Schedule();

         Entities.WithAll<FireIndex>()
             .ForEach( (ref FireIndex fireIndex, ref URPMaterialPropertyBaseColor colorComponent) =>
             {
                 float intensity = heatmapBuffer[fireIndex.index];
                 colorComponent.Value = new float4( intensity );
             })
             .Schedule();
    }

    static void HeatAdjacents(ref DynamicBuffer<HeatMapTemperature> buffer,int tileIndex, int width, float deltaTime)
    {
        //check out of bounds
        //(x-1, z-1), (x, z-1), (x+1, z-1) 
        //(x-1, z  ), *(x, z)*, (x+1, z  ) 
        //(x-1, z+1), (x, z+1), (x+1, z+1) 
        
        int2[] checkAdjacents = new int2[]
        {
            new int2(-1,-1), new int2(0,-1), new int2(1,-1), 
            new int2(-1,0),  new int2(1,0), 
            new int2(-1,+1), new int2(0,+1), new int2(1,1), 
        };

        for (int iCheck = 0; iCheck < checkAdjacents.Length; iCheck++)
        {
            int x = tileIndex + checkAdjacents[iCheck].x;
            int z = tileIndex + checkAdjacents[iCheck].y;

            bool outOfBounce = (x < 0 || x > width-1 || z < 0 || z > width-1);
            int adjacentIndex = GetTileIndex(x, z, width);

            if (!outOfBounce && buffer[adjacentIndex] < 0.2f)
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
    
    int2 GetTileCoordinate(int index , int width)
    {
        int x = index / width;
        int z = index % width;
        return new int2(x, z);
    }
}

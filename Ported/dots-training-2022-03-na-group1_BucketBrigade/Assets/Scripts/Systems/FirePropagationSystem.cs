using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FirePropagationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // var heatmap = GetSingletonEntity<HeatMapTemperature>();
        // DynamicBuffer<HeatMapTemperature> heatmapBuffer = EntityManager.GetBuffer<HeatMapTemperature>(heatmap);
        //
        // var intDynamicBuffer = heatmapBuffer.Reinterpret<float>();
        // for (int i = 0; i < heatmapBuffer.Length; i++)
        // {
        //     // ...
        //     intDynamicBuffer[i] = 0.5f;
        // }
        
         Entities
             .ForEach( (ref DynamicBuffer<HeatMapTemperature> heatmapBuffer) =>
             {
                 var buffer = heatmapBuffer.Reinterpret<float>();
                 for (var i = 0; i < buffer.Length; i++)
                 {
                     if(buffer[i] >= 0.2f)
                         buffer[i] += Time.DeltaTime;

                     if (buffer[i] >= 1f)
                         buffer[i] = 1f;
                 }
             })
             .Schedule();
    }

    int2 GetTileCoordinate(int index , int width)
    {
        int x = index / width;
        int y = index % width;
        return new int2(x, y);
    }
}

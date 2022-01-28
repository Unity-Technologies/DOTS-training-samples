using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class RenderWaterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<Lake, OriginalLake>()
            .ForEach((ref NonUniformScale scale, in Lake lake, in OriginalLake originalLake) =>
            {
                scale.Value = math.lerp(float3.zero, originalLake.Scale, lake.Volume / originalLake.Volume);
            }).ScheduleParallel();
    }
}
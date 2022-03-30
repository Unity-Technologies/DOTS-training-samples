using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class WaterSpotScaleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithNone<BucketTag>()
            .ForEach((ref NonUniformScale scale, in Volume waterCapacity) =>
            {
                var size = math.sqrt(waterCapacity.Value) * 0.1f;
                scale.Value = new float3(size, 0.02f, size);
            }).ScheduleParallel();
    }
}

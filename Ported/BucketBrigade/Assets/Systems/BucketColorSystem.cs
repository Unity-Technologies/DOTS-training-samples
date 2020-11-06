
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Collections;


public class BucketColorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref URPMaterialPropertyBaseColor color, in EmptyBucket b) =>
            {
                color.Value = new float4(0.28f, 0.28f, 0.28f, 1.0f);
            }).ScheduleParallel();

        Entities
            .ForEach((ref URPMaterialPropertyBaseColor color, in FullBucket b) =>
            {
                color.Value = new float4(0.02f, 0.58f, 0.81f, 1.0f);
            }).ScheduleParallel();
    }
}
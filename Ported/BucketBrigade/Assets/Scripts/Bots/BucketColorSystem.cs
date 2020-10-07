using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BucketColorSystem : SystemBase
{
    protected override void OnUpdate()
    {
        FireSimulation fireSim = GetSingleton<FireSimulation>();
        
        Entities
            .WithName("SetBucketColor")
            .ForEach((ref BaseColor color, in Volume volume) =>
            {
                Color col = Color.Lerp(fireSim.bucketColorEmpty, fireSim.bucketColorFull, volume.Value);
                color.Value = new float4(col.r, col.g, col.b, col.a);
            }).ScheduleParallel();
    }
}

public class BucketScaleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        FireSimulation fireSim = GetSingleton<FireSimulation>();
        
        Entities
            .WithName("SetScaleColor")
            .ForEach((ref MyScale scale, in Volume volume) =>
            {
                float s = math.lerp(fireSim.bucketSizeEmpty, fireSim.bucketSizeFull, volume.Value);
                scale.Value = s;
            }).ScheduleParallel();
    }
}
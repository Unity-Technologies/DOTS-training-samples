using Unity.Entities;
using Unity.Mathematics;

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
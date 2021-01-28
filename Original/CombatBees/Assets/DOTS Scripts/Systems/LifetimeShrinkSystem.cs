using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class LifetimeShrinkSystem: SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithAll<BeeCorpseTag>()
            .ForEach((ref NonUniformScale scale, in Lifetime lifetime) =>
            {
                scale.Value = new float3(lifetime.NormalizedTimeRemaining);
            }).Run();
    }
}

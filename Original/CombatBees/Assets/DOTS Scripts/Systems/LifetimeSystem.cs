using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class LifetimeSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        // Update the lifetime
        Entities.ForEach((ref Lifetime lifetime) =>
        {
            lifetime.NormalizedTimeRemaining -= math.clamp(lifetime.NormalizedDecaySpeed * deltaTime, 0, 1);
        }).ScheduleParallel();
        
        // Clean all dead entities
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var pEcb = ecb.AsParallelWriter();
        Entities.ForEach((Entity e, int entityInQueryIndex, in Lifetime lifetime) =>
        {
            if (lifetime.NormalizedTimeRemaining <= 0)
            {
                pEcb.DestroyEntity(entityInQueryIndex, e);
            }
        }).ScheduleParallel();
        
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

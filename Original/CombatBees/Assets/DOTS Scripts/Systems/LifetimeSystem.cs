using Unity.Collections;
using Unity.Entities;

public class LifetimeSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        // Update the lifetime
        Entities.ForEach((ref Lifetime lifetime) =>
        {
            lifetime.NormalizedTimeRemaining -= lifetime.NormalizedDecaySpeed * deltaTime;
        }).ScheduleParallel();

        
        // Clean all dead entities
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var pEcb = ecb.AsParallelWriter();
        Entities.ForEach((Entity e, int entityInQueryIndex, in Lifetime lifetime) =>
        {
            if (lifetime.NormalizedTimeRemaining < 0)
            {
                pEcb.DestroyEntity(entityInQueryIndex, e);
            }
        }).ScheduleParallel();
        
        Dependency.Complete();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

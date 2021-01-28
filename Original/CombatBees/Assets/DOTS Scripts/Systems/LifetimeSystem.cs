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
        }).Run();

        // Clean all dead entities
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.ForEach((Entity e, in Lifetime lifetime) =>
        {
            if (lifetime.NormalizedTimeRemaining < 0)
            {
                ecb.DestroyEntity(e);
            }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}

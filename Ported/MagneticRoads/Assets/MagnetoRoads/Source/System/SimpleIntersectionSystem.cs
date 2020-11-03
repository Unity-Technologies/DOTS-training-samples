using Unity.Collections;
using Unity.Entities;

public class SimpleIntersectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        float elapsedTime = Time.fixedDeltaTime;

        Entities
            .ForEach((Entity entity, ref SimpleIntersection simpleIntersection) =>
            {
                
            }).Run();

        ecb.Playback(EntityManager);
    }
}
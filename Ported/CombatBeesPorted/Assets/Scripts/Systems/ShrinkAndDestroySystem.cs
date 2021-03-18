using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ShrinkAndDestroySystem: SystemBase
{
    
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .ForEach((Entity entity, ref ShrinkAndDestroy shrinkAndDestroy, ref NonUniformScale scale) =>
            {
                shrinkAndDestroy.age += deltaTime;

                if (shrinkAndDestroy.age > shrinkAndDestroy.lifetime)
                {
                    commandBuffer.DestroyEntity(entity);
                }

                var scaleCof = 1.0f - math.clamp(shrinkAndDestroy.age / shrinkAndDestroy.lifetime, 0, 1);
                scale.Value *= scaleCof;
            }).Run();
        
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
    }
    
}
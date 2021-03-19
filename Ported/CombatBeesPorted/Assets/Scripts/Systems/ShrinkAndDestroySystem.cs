using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ShrinkAndDestroySystem: SystemBase
{
    
    EndInitializationEntityCommandBufferSystem endSim;    

    protected override void OnCreate()
    {
        base.OnCreate();
        endSim = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        var commandBuffer = endSim.CreateCommandBuffer();
        
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
            }).Schedule();
            
        endSim.AddJobHandleForProducer(Dependency);
    }
    
}
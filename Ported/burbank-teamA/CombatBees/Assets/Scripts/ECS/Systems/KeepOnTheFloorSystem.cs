using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public class KeepOnTheFloorSystem : JobComponentSystem
{

    BeginInitializationEntityCommandBufferSystem bufferSystem;
    
    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();

    }


    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var gameBounds = GetSingleton<GameBounds>();
        var commandBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent();

  
        return Entities.WithAny<ResourceTag, ParticleTag>()
            .ForEach((Entity e, ref Translation t, ref Velocity velocity, ref GravityMultiplier gm) =>
            {

                if (t.Value.y < -gameBounds.Value.y)
                {
                    gm.Value = 0;
                    velocity.Value = new float3();
                }

                if (math.abs(t.Value.x) > gameBounds.Value.x)
                {
                    velocity.Value.x = 0;
                }

                if (math.abs(t.Value.z) > gameBounds.Value.z)
                {
                    velocity.Value.z = 0;
                }

            })
            .Schedule(inputDeps);
    }
}

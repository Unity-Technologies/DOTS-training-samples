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
        var gameBounds = GetSingleton<GameBounds>().Value;
        var commandBuffer = bufferSystem.CreateCommandBuffer().ToConcurrent();
        //var beePrefab = GetSingleton<BeeManager>().Value;


        return Entities.WithoutBurst().WithAny<ResourceTag, ParticleTag>()
            .ForEach((Entity e, ref Translation t, ref Velocity velocity, ref GravityMultiplier gm) =>
            {

                if (t.Value.y < -gameBounds.y)
                {

                    if (math.abs(t.Value.x) > gameBounds.x*0.8)
                    {
                        /*var spawnedBee = commandBuffer.Instantiate(0, beePrefab);
                        commandBuffer.SetComponent(0, spawnedBee, new Translation
                        {
                            Value = t.Value
                        });*/

                        commandBuffer.DestroyEntity(0, e);
                    }

                    gm.Value = 0;
                    velocity.Value = new float3();

                }

                if (math.abs(t.Value.x) > gameBounds.x)
                {
                    velocity.Value.x = 0;
                }

                if (math.abs(t.Value.z) > gameBounds.z)
                {
                    velocity.Value.z = 0;
                }

            })
            .Schedule(inputDeps);
    }
}

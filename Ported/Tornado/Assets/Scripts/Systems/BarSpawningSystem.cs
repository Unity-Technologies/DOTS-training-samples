using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BarSpawningSystem : SystemBase
{
    // A command buffer system executes command buffers in its own OnUpdate
    public EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BarSpawner>();
        // Get the command buffer system
        CommandBufferSystem
            = World.DefaultGameObjectInjectionWorld.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // The command buffer to record commands,
        // which are executed by the command buffer system later in the frame
        EntityCommandBuffer.ParallelWriter commandBuffer
            = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        var spawner = GetSingleton<BarSpawner>();

        Entities.ForEach(( DynamicBuffer<Constraint> bufferConstraint)  =>
        {
            for (int i = 0; i < bufferConstraint.Length; i++)
            {
                var instance = ecb.Instantiate(spawner.barPrefab);
                var posA = GetComponent<Translation>(bufferConstraint[i].pointA).Value;
                var posB = GetComponent<Translation>(bufferConstraint[i].pointB).Value;
                var translation = new Translation();
                var rotation = new Rotation();
                var scale = new NonUniformScale();
                
                translation.Value = (posA + posB) * 0.5f;
                rotation.Value = Quaternion.LookRotation(((Vector3) (posA - posB)).normalized);
                scale.Value = new float3(0.2f, 0.2f, Vector3.Distance(posA, posB));

                ecb.SetComponent(instance, rotation);
                ecb.SetComponent(instance, translation);
                ecb.AddComponent(instance, scale);
            }
        }).Run();

        ecb.Playback(EntityManager);

        Enabled = false;
    }
}

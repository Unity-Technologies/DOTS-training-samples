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

        Entities.ForEach( ( Entity entity, int entityInQueryIndex, in Constraint constraint )  =>
        {
            var instance = ecb.Instantiate(spawner.barPrefab);

            var posA = EntityManager.GetComponentData<Translation>(constraint.pointA).Value;
            var posB = EntityManager.GetComponentData<Translation>(constraint.pointB).Value;

            var translation = new Translation();
            translation.Value = (posA + posB) * 0.5f;

            var rotation = new Rotation();
            rotation.Value = Quaternion.LookRotation(((Vector3)(posA - posB)).normalized);

            var scale = new NonUniformScale();
            scale.Value = new float3(0.2f, 0.2f, Vector3.Distance(posA, posB) );

            ecb.SetComponent(instance, rotation);
            ecb.SetComponent(instance, translation);
            ecb.AddComponent(instance, scale);
        }).Run();

        ecb.Playback(EntityManager);

        Enabled = false;
    }
}

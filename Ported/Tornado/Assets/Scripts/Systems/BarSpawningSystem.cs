using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(CreateBuildingSystem))]
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

        Random random = new Random(1234);

        Entities.WithAll<Building>().ForEach( ( Entity entity)  =>
        {
            var bufferConstraint = GetBuffer<Constraint>(entity);

            Debug.Log($"Contraints : {bufferConstraint.Length}");



            for (int i = 0; i < bufferConstraint.Length; i++)
            {
                var instance = ecb.Instantiate(spawner.barPrefab);
                var posA = GetComponent<Translation>(bufferConstraint[i].pointA).Value;
                var posB = GetComponent<Translation>(bufferConstraint[i].pointB).Value;
                float3 delta = posA - posB;
                delta = math.abs(math.normalize(delta));
                float thickness = (delta.y > 0.95) ? 0.2f : (delta.y < 0.05f) ? 0.15f : 0.1f;

                var translation = new Translation();
                var rotation = new Rotation();
                var scale = new NonUniformScale();
                
                translation.Value = (posA + posB) * 0.5f;
                rotation.Value = Quaternion.LookRotation(((Vector3) (posA - posB)).normalized);
                scale.Value = new float3(thickness, thickness, Vector3.Distance(posA, posB));

                spawner.color = new float4(1.0f) * 0.5f * (1f - delta.y) * random.NextFloat(.7f, 1f);

                ecb.SetComponent(instance, rotation);
                ecb.SetComponent(instance, translation);
                ecb.AddComponent(instance, scale);
                ecb.AddComponent(instance, new HDRPMaterialPropertyBaseColor { Value = spawner.color });

                bufferConstraint[i].AssignBarTransform(instance);
            }
        }).Run();

        ecb.Playback( EntityManager );

        Enabled = false;
    }
}

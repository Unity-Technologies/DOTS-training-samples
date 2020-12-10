using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
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

        query = GetEntityQuery(typeof(Building));
    }

    EntityQuery query;

    protected override void OnUpdate()
    {
        // The command buffer to record commands,
        // which are executed by the command buffer system later in the frame
        EntityCommandBuffer.ParallelWriter commandBuffer
            = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var spawner = GetSingleton<BarSpawner>();

        Random random = new Random(1234);

        var buildings = query.ToEntityArray(Allocator.Temp);

        foreach (var buildingEntity in buildings)
        {
            var bars = EntityManager.Instantiate(spawner.barPrefab, GetBuffer<Constraint>(buildingEntity).Length, Allocator.Temp);
            var bufferConstraint = GetBuffer<Constraint>(buildingEntity);

            for (int i = 0; i < bars.Length; i++)
            {
                var constraint = bufferConstraint[i];
                constraint.AssignBarTransform(bars[i]);
                bufferConstraint[i] = constraint;
            }
        }

        Entities.WithAll<Building>().ForEach((Entity entity) =>
        {
            var bufferConstraint = GetBuffer<Constraint>(entity);

            for (int i = 0; i < bufferConstraint.Length; i++)
            {
                var instance = bufferConstraint[i].barTransform;
                var posA = GetComponent<Translation>(bufferConstraint[i].pointA).Value;
                var posB = GetComponent<Translation>(bufferConstraint[i].pointB).Value;
                float3 delta = posA - posB;
                delta = math.abs(math.normalize(delta));
                float thickness = (delta.y > 0.95) ? 0.2f : (delta.y < 0.05f) ? 0.15f : 0.1f;

                var translation = new Translation();
                var rotation = new Rotation();
                var scale = new NonUniformScale();

                translation.Value = (posA + posB) * 0.5f;
                rotation.Value = Quaternion.LookRotation(((Vector3)(posA - posB)).normalized);
                scale.Value = new float3(thickness, thickness, Vector3.Distance(posA, posB));

                ecb.SetComponent(instance, rotation);
                ecb.SetComponent(instance, translation);
                ecb.AddComponent(instance, scale);
                ecb.AddComponent(instance, new HDRPMaterialPropertyBaseColor { Value = new float4(1.0f) * 0.5f * (1f - delta.y) * random.NextFloat(.7f, 1f) });
            }
        }).Run();

        ecb.Playback(EntityManager);

        Enabled = false;
    }
}

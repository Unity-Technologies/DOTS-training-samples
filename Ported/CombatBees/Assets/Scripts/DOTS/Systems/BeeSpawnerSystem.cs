using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BeeSpawnerSystem : SystemBase
{
    private EntityQuery m_baseQuery;
    Unity.Mathematics.Random m_Random = new Unity.Mathematics.Random(0x5716318);
    private EntityCommandBufferSystem m_ECBSystem;


    protected override void OnCreate()
    {
        m_baseQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<FieldInfo>()
            },
            Any = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<TeamTwo>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var baseFields = m_baseQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var baseHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, baseHandle); 

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var random = m_Random;

        var maxSpawnSpeed = BeeManager.Instance.maxSpawnSpeed;
        var minBeeSize = BeeManager.Instance.minBeeSize;
        var maxBeeSize = BeeManager.Instance.maxBeeSize;

        Entities
            .WithDeallocateOnJobCompletion(baseFields)
            .ForEach((int entityInQueryIndex, Entity entity, in BeeSpawner spawner, in LocalToWorld ltw) =>
            {
                for (int i = 0; i < baseFields.Length; i++)
                {
                    var baseField = baseFields[i]; 

                    float xCenter = baseField.Bounds.Center.x;
                    float xExtents = baseField.Bounds.Extents.x;
                    float yCenter = baseField.Bounds.Center.y;
                    float yExtents = baseField.Bounds.Extents.y;
                    float zCenter = baseField.Bounds.Center.z;
                    float zExtents = baseField.Bounds.Extents.z;

                    for (int x = 0; x < spawner.BeeCount; ++x)
                    {
                        var xVal = random.NextFloat(xCenter - xExtents, xCenter + xExtents);
                        var yVal = random.NextFloat(yCenter - yExtents, yCenter + yExtents);
                        var zVal = random.NextFloat(zCenter - zExtents, zCenter + zExtents);

                        var instance = ecb.Instantiate(entityInQueryIndex, spawner.Prefab);

                        ecb.SetComponent(entityInQueryIndex, instance, new Translation
                        {
                            Value = new float3(xVal, yVal, zVal)
                        });

                        ecb.AddComponent(entityInQueryIndex, instance, new Target());

                        if (i == 0)
                        {
                            ecb.AddComponent(entityInQueryIndex, instance, new TeamOne());
                        }
                        else
                        {
                            ecb.AddComponent(entityInQueryIndex, instance, new TeamTwo());
                        }

                        ecb.SetComponent(entityInQueryIndex, instance, new BeeColor { Value = new float4(baseField.TeamColor.r, baseField.TeamColor.g, baseField.TeamColor.b, baseField.TeamColor.a) });

                        // Random start velocity
                        //ecb.SetComponent(entityInQueryIndex, instance, new Velocity() { Value = random.NextFloat3Direction() * maxSpawnSpeed });

                        // Random size
                        ecb.SetComponent(entityInQueryIndex, instance, new Size() { Value = random.NextFloat(minBeeSize, maxBeeSize) });

                        // HACK
                        ecb.AddComponent(entityInQueryIndex, instance, new KillBee { Time = random.NextFloat(1, 3) });
                    }
                }

                ecb.DestroyEntity(entityInQueryIndex, entity);
            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
        Enabled = false;
    }
}

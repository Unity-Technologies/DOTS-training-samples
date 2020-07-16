using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ResourceSpawnerSystem : SystemBase
{
    private EntityQuery m_MainFieldQuery;
    private EntityCommandBufferSystem m_ECBSystem;
    private Unity.Mathematics.Random m_Random;

    protected override void OnCreate()
    {
        m_MainFieldQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<FieldInfo>()
            }, 
            None = new[]
            {
                ComponentType.ReadOnly<TeamOne>(),
                ComponentType.ReadOnly<TeamTwo>()
            }
        });

        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        m_Random = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks); 
    }

    protected override void OnUpdate()
    {
        var mainFields = m_MainFieldQuery.ToComponentDataArrayAsync<FieldInfo>(Unity.Collections.Allocator.TempJob, out var mainFieldHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, mainFieldHandle);

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var random = m_Random; 

        Entities
            .WithDeallocateOnJobCompletion(mainFields)
            .ForEach((int entityInQueryIndex, Entity entity, in Spawner spawner, in LocalToWorld ltw) =>
            {
                var mainField = mainFields[0];
                var center = mainField.Bounds.Center;

                for (int x = 0; x < spawner.Count; ++x)
                {
                    var instance = ecb.Instantiate(entityInQueryIndex, spawner.Prefab);
                    ecb.SetComponent(entityInQueryIndex, instance, new Translation
                    {
                        Value = center + new float3(math.sin(x), 0, math.cos(x)) * mainField.Bounds.Extents.z * random.NextFloat()
                    });
                }

                ecb.DestroyEntity(entityInQueryIndex, entity);
            }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
        Enabled = false;
    }
}

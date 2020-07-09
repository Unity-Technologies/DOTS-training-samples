using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class PlantGrowingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private EntityQuery m_SpawnerQuery;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        m_SpawnerQuery = GetEntityQuery(new EntityQueryDesc { All = new[] { ComponentType.ReadOnly<AutoFarmers.PlantSpawner>() } });
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent();

        float deltaTime = Time.DeltaTime;

        NativeArray<AutoFarmers.PlantSpawner> plantSpawnerArray = m_SpawnerQuery.ToComponentDataArrayAsync<AutoFarmers.PlantSpawner>(Allocator.TempJob, out var spawnerHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, spawnerHandle);

        Entities
            .WithDeallocateOnJobCompletion(plantSpawnerArray)
            .WithAll<Plant_Tag>()
            .WithNone<FullyGrownPlant_Tag>().ForEach((int entityInQueryIndex, Entity entity, ref Health health) =>
        {
            health.Value += deltaTime * plantSpawnerArray[0].GrowRate;
            if (health.Value >= 1.0f)
            {
                ecb.AddComponent<FullyGrownPlant_Tag>(entityInQueryIndex, entity, new FullyGrownPlant_Tag());
            }
        }).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

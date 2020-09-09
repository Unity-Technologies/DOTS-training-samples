using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var dt = UnityEngine.Time.deltaTime;
        
        Entities.ForEach((int entityInQueryIndex, Entity spawnerEntity, ref Spawner spawner, ref PositionXZ positionXz) =>
        {
            if (spawner.TotalSpawned >= spawner.Max)
                return;
            
            spawner.Counter += dt;
            if (!(spawner.Counter > spawner.Frequency)) return;
            
            spawner.Counter -= spawner.Frequency;
            var instance = ecb.Instantiate(entityInQueryIndex, spawner.Prefab);
            ecb.SetComponent(entityInQueryIndex, instance, new PositionXZ() {Value = positionXz.Value});
            ecb.AddComponent(entityInQueryIndex, instance, new Speed(){Value = 2});

            spawner.TotalSpawned++;
        }).ScheduleParallel();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
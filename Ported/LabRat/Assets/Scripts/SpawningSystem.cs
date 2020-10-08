using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawningSystem: SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithAll<Timer, SpawnPoint>().ForEach((Entity entity, int entityInQueryIndex, in Timer timer) =>
        {
            if(timer.Value <= 0f)
                ecb.RemoveComponent<Timer>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        var seed = (uint)DateTime.Now.Ticks;
        Entities
            .WithAll<SpawnPoint, Translation>()
            .WithNone<Timer>()
            .ForEach((Entity entity, int entityInQueryIndex, ref SpawnPoint spawnPoint, in Translation translation) =>
            {
                var rand = new Unity.Mathematics.Random((uint)(seed + entityInQueryIndex));

                if (spawnPoint.spawnCount > 0)
                {
                    var instance = ecb.Instantiate(entityInQueryIndex, spawnPoint.spawnPrefab);

                    ecb.AddComponent(entityInQueryIndex, instance, new Position { Value = new float2(translation.Value.x, translation.Value.z)});
                    ecb.AddComponent(entityInQueryIndex, instance, new Speed { Value = rand.NextFloat(spawnPoint.speedRange.x, spawnPoint.speedRange.y) });
                    ecb.AddComponent(entityInQueryIndex, instance, new Direction { Value = spawnPoint.direction});
                    
                    ecb.AddComponent(entityInQueryIndex, entity, new Timer() { Value = spawnPoint.spawnFrequency });

                    spawnPoint.spawnCount -= 1;
                }
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

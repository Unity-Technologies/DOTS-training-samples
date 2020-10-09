using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.PlayerLoop;

public class SpawningSystem : SystemBase
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
            if (timer.Value <= 0f)
                ecb.RemoveComponent<Timer>(entityInQueryIndex, entity);
        }).ScheduleParallel();

        var seed = (uint) DateTime.Now.Ticks;
        Entities
            .WithAll<SpawnPoint, Translation>()
            .WithNone<Timer>()
            .ForEach((Entity entity, int entityInQueryIndex, ref SpawnPoint spawnPoint,
                in DynamicBuffer<SpawnType> types, in Translation translation) =>
            {
                var rand = new Unity.Mathematics.Random((uint) (seed + entityInQueryIndex));

                var instance = ecb.Instantiate(entityInQueryIndex, types[spawnPoint.spawnType].spawnPrefab);

                ecb.AddComponent(entityInQueryIndex, instance,
                    new Position {Value = new float2(translation.Value.x, translation.Value.z)});
                ecb.AddComponent(entityInQueryIndex, instance,
                    new Speed
                    {
                        Value = rand.NextFloat(types[spawnPoint.spawnType].speedRange.x,
                            types[spawnPoint.spawnType].speedRange.y)
                    });
                ecb.AddComponent(entityInQueryIndex, instance, new Direction {Value = spawnPoint.direction});
                ecb.AddComponent(entityInQueryIndex, instance,
                    new TileCoord() {Value = new int2((int) translation.Value.x, (int) translation.Value.z)});
                ecb.AddComponent(entityInQueryIndex, instance, new TileCheckTag());

                spawnPoint.spawnCount -= 1;
                if (spawnPoint.spawnCount == 0)
                {
                    spawnPoint.spawnType = (spawnPoint.spawnType + 1) % types.Length;
                    spawnPoint.spawnCount = types[spawnPoint.spawnType].spawnMax;
                    ecb.AddComponent(entityInQueryIndex, entity, new Timer() {Value = types[spawnPoint.spawnType].spawnDelay});
                }
                else
                {
                    ecb.AddComponent(entityInQueryIndex, entity,
                        new Timer() {Value = types[spawnPoint.spawnType].spawnFrequency});
                }
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
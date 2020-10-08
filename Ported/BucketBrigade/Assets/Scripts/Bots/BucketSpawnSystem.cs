using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BucketSpawnSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Random rand = new Random((uint)System.DateTime.UtcNow.Millisecond);
        
        Entities
            .WithName("SpawnBuckets")
            .ForEach((Entity spawnerEntity, int entityInQueryIndex,
                in BucketSpawner spawner) =>
            {
                for (int i = 0; i < spawner.numberBuckets; i++)
                {
                    Entity e = ecb.Instantiate(i, spawner.bucketPrefab);

                    float2 pos = rand.NextFloat2() * spawner.spawnRadius - new float2(spawner.spawnRadius * 0.5f) + spawner.spawnCenter;
                    ecb.AddComponent(i, e, new Pos { Value = pos });
                    
                    // This bot is being created with a Pos and it never rotates. Could we do this in the Prefab though?
                    ecb.RemoveComponent<Translation>(i, e);
                    ecb.RemoveComponent<Rotation>(i, e);
                    ecb.RemoveComponent<NonUniformScale>(i, e);
                }
                
                ecb.DestroyEntity(spawner.numberBuckets, spawnerEntity);
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
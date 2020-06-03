using Unity.Entities;

class SpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        var ecb = commandBufferSystem.CreateCommandBuffer();

        float dt = Time.DeltaTime;
        
        Entities.ForEach((ref SpawnerInstance instance, in SpawnerInfo info, in Position2D position, in Direction2D direction) =>
        {
            instance.Time += dt;
            instance.AlternateSpawnTime += dt;
            Entity toSpawn = Entity.Null;

            if(info.AlternateSpawnFrequency != 0 && instance.AlternateSpawnTime >= info.AlternateSpawnFrequency)
            {
                instance.AlternateSpawnTime -= info.AlternateSpawnFrequency;
                instance.Time = 0; // Reset the default spawner time so it doesn't spawn randomly another entity straight away
                toSpawn = ecb.Instantiate(info.AlternatePrefab);
            }
            else if(info.Frequency != 0 && instance.Time >= info.Frequency)
            {
                instance.Time -= info.Frequency;
                toSpawn = ecb.Instantiate( info.Prefab);
            }

            if(toSpawn != Entity.Null)
            {
                ecb.SetComponent(toSpawn, new Position2D { Value = position.Value });
                ecb.SetComponent(toSpawn, new Direction2D { Value = direction.Value });
            }
        })
        .WithName("UpdateSpawners")
        .Schedule();

        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
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
            if(instance.Time >= info.Frequency)
            {
                instance.Time -= info.Frequency;

                Entity e = ecb.Instantiate( info.Prefab);
                ecb.SetComponent(e, new Position2D { Value = position.Value});
                ecb.SetComponent(e, new Direction2D { Value = direction.Value });
            }
        })
        .WithName("UpdateSpawners")
        .Schedule();

        commandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
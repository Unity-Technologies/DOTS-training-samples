using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class SpawnerSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        var dt = UnityEngine.Time.deltaTime;
        
        Entities.ForEach((Entity spawnerEntity, ref Spawner spawner) =>
        {
            if (spawner.TotalSpawned >= spawner.Max)
                return;
            
            spawner.Counter += dt;
            if (!(spawner.Counter > spawner.Frequency)) return;
            
            spawner.Counter -= spawner.Frequency;
            var instance = ecb.Instantiate(spawner.Prefab);
            var translation = new float3(spawner.TotalSpawned * 2, 0, 0);
            ecb.SetComponent(instance, new Translation {Value = translation});

            spawner.TotalSpawned++;


        }).Schedule();

        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
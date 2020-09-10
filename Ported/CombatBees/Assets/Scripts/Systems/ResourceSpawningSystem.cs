using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ResourceSpawningSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        
        Entities.ForEach((Entity spawnerEntity, in ResourceSpawner spawner, in Translation spawnerTranslation) =>
        {
            for (int z = 0; z < spawner.SizeZ; ++z)
            {
                for (int x = 0; x < spawner.SizeX; ++x)
                {
                    var instance = ecb.Instantiate(spawner.ResourcePrefab);
                    var translation = spawnerTranslation.Value + new float3(x - (spawner.SizeX - 1) / 2f, 0, z - (spawner.SizeZ - 1) / 2f);
                    //translation *= brickSize * 1.1f;
                    ecb.SetComponent(instance, new Translation {Value = translation});
                }
            }
            
            ecb.DestroyEntity(spawnerEntity);
        }).Schedule();
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
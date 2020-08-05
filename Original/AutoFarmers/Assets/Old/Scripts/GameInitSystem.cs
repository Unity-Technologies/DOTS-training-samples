using Unity.Entities;
using Unity.Transforms;

public class GameInitSystem : SystemBase
{
    // private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        // m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        Entities.WithStructuralChanges().ForEach((Entity entity, in GroundData groundData) =>
        {
            for (int y = 0; y < 20; ++y)
            {
                for (int x = 0; x < 20; ++x)
                {
                    var tile = EntityManager.Instantiate(groundData.groundEntity);
                    EntityManager.RemoveComponent<Translation>(tile);
                    EntityManager.RemoveComponent<Rotation>(tile);
                    EntityManager.RemoveComponent<Scale>(tile);
                    EntityManager.RemoveComponent<NonUniformScale>(tile);
                    EntityManager.AddComponentData<Position2D>(tile, new Position2D{ position = new Unity.Mathematics.float2(x, y)});

                    // var instance = ecb.Instantiate(spawner.BrickPrefab);
                    // var translation = new float2(x, y);
                    // ecb.SetComponent(instance, new Position2D {position = translation});
                }
            }
            EntityManager.RemoveComponent<GroundData>(entity);
            // ecb.DestroyEntity(entity);
        }).Run();
        // m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
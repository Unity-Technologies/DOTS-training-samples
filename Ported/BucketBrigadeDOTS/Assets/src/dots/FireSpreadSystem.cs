using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FireSpreadSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        //var fireGridEntity = GetSingletonEntity<FireGridSettings>();
        //var bufferRef = EntityManager.GetBuffer<FireCell>(fireGridEntity);
        /*
        Entities
        .ForEach((int entityInQueryIndex) =>
        {

        }).Schedule();
        */
        //m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}

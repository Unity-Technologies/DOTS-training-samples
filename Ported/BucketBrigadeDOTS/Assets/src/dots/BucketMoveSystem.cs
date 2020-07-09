using Unity.Entities;
using Unity.Mathematics;

public class BucketMoveSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((int entityInQueryIndex, Entity entity, in WaterBucketID waterBucketID, in Translation2D translation) =>
        {
            ecb.SetComponent(entityInQueryIndex, waterBucketID.Value, translation);
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

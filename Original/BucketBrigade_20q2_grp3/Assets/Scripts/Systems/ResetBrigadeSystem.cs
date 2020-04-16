using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public struct Reset : IComponentData
{
    public double ResetTime;
}

public class ResetSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities.ForEach((int entityInQueryIndex, Entity e, Reset r) =>
        {
            if (time > r.ResetTime)
            {
                if (HasComponent<ResourceSourcePosition>(e))
                {
                    var resourceId = GetComponent<ResourceSourcePosition>(e).Id;
                    ecb.RemoveComponent<ResourceClaimed>(entityInQueryIndex, resourceId);
                }
                ecb.RemoveComponent<ResourceSourcePosition>(entityInQueryIndex, e);
                ecb.RemoveComponent<ResourceTargetPosition>(entityInQueryIndex, e);
            }
        }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

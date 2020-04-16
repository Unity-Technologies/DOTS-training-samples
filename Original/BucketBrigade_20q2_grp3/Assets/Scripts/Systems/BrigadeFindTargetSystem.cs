using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public class BrigadeFindTargetSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    Random random = new Random(775453);
    protected override void OnUpdate()
    {
        var rand = random;
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .WithNone<ResourceTargetPosition>()
            .ForEach((int entityInQueryIndex, Entity e, in BrigadeLine line) =>
            {
                ecb.RemoveComponent<BrigadeLineEstablished>(entityInQueryIndex, e);
                ecb.AddComponent(entityInQueryIndex, e, new ResourceTargetPosition() { Value = rand.NextFloat2(new float2(0, 0), new float2(100, 100)) });
            }).ScheduleParallel();
        random = rand;
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
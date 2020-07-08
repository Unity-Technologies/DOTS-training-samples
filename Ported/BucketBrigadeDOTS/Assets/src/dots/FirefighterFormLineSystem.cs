using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class FirefighterFormLineSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        float2 src = new float2(0.0f, 0.0f);
        float2 dst = new float2(10.0f, 10.0f);
        
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.WithNone<Target>().ForEach((int entityInQueryIndex, Entity entity, Firefighter firefighter, FirefighterPositionInLine positionInLine, in Translation2D translation) =>
        {
            float2 pos = (dst - src) * positionInLine.Value + src;
            ecb.AddComponent<Target>(entityInQueryIndex, entity, new Target{ Value = pos });
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

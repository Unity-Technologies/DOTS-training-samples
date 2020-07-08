using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class FirefighterRandomTargetSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.WithNone<Target>().ForEach((int entityInQueryIndex, Entity entity, Firefighter firefighter, in Translation2D translation) =>
        {
            var rand = new Random((uint)((translation.Value.x * 10000 + translation.Value.y * 100 + 1 )));
            float2 pos = rand.NextFloat2(0, 10) - 5;
            ecb.AddComponent<Target>(entityInQueryIndex, entity, new Target{ Value = pos });
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

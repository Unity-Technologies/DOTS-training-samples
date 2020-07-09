using System.ComponentModel.Design.Serialization;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class FirefighterMoveToTargetSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        var speed = 2.0f;
        var deltaTime = Time.DeltaTime;

        Entities.ForEach((int entityInQueryIndex, Entity entity, ref Translation2D translation, in Target target) =>
        {
            float2 fromTo = target.Value - translation.Value;
            float dist = math.length(fromTo);
            var step = speed * deltaTime;
            if (dist < step)
            {
                translation.Value = target.Value;
                ecb.RemoveComponent<Target>(entityInQueryIndex, entity);
            }
            else
            {
                translation.Value += step * fromTo / dist;
            }
        }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

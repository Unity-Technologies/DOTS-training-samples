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
        var ecb = m_ECBSystem.CreateCommandBuffer();
        var speed = 0.01f;

        Entities.ForEach((Entity entity, ref Translation2D translation, in Target target) =>
        {
            float2 fromTo = target.Value - translation.Value;
            float dist = math.length(fromTo);
            if (dist < 0.1f)
                ecb.RemoveComponent<Target>(entity);
            else
                translation.Value += fromTo * speed / dist;
        }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

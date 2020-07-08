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

        Entities.ForEach((Entity entity, ref Translation2D translation, in Target target) =>
        {
            translation.Value += math.normalize(target.Value - translation.Value) * 0.01f;
            // ecb.AddComponent<Target>(entity, new Target{ Value = pos });
        }).Schedule();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

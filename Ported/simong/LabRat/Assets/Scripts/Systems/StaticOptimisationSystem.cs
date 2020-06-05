using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Streaming;
using Unity.Transforms;

public struct StaticOptimisationRequest : IComponentData
{}

public class StaticOptimisationSystem : SystemBase
{
    private EntityQuery m_requestQuery;

    protected override void OnCreate()
    {
        m_requestQuery = GetEntityQuery(new EntityQueryDesc()
        {
            All = new[] {ComponentType.ReadWrite<StaticOptimisationRequest>()},
        });

        RequireForUpdate(m_requestQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();
        ecb.RemoveComponent<Child>(m_requestQuery);
        ecb.RemoveComponent<StaticOptimisationRequest>(m_requestQuery);
    }
}
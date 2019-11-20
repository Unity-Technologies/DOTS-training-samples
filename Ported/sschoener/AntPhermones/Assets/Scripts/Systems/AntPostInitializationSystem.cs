using System;
using Unity.Entities;

[UpdateInGroup((typeof(InitializationSystemGroup)))]
public class AntPostInitializationSystem : ComponentSystem
{
    EntityQuery m_RemoveUninitTagQuery;
    protected override void OnCreate()
    {
        base.OnCreate();
        m_RemoveUninitTagQuery = GetEntityQuery(ComponentType.ReadWrite<UninitializedTagComponent>());
    }

    protected override void OnUpdate()
    {
        EntityManager.RemoveComponent<UninitializedTagComponent>(m_RemoveUninitTagQuery);
    }
}

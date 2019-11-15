using System;
using Unity.Entities;

namespace Systems {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class RemoveUninitializedTagSystem : ComponentSystem
    {
        EntityQuery m_UninitializedTagQuery;
        protected override void OnCreate()
        {
            base.OnCreate();
            m_UninitializedTagQuery = GetEntityQuery(ComponentType.ReadWrite<UninitializedTagComponent>());
        }

        protected override void OnUpdate()
        {
            EntityManager.RemoveComponent<UninitializedTagComponent>(m_UninitializedTagQuery);
        }
    }
}

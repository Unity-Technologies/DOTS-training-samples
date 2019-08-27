using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
    internal readonly unsafe struct EntityToComponentMap<TComponentData> : IDisposable
        where TComponentData : unmanaged, IComponentData
    {
        private readonly int m_TypeIndex;
        [NativeDisableContainerSafetyRestriction] private readonly NativeHashMap<Entity, TComponentData> m_HashMap;
        [NativeDisableUnsafePtrRestriction] private readonly EntityComponentStore* m_EntityComponentStore;

        public EntityToComponentMap(EntityComponentStore* entityComponentStore, Allocator label)
        {
            m_TypeIndex = TypeManager.GetTypeIndex<TComponentData>();
            m_EntityComponentStore = entityComponentStore;
            m_HashMap = new NativeHashMap<Entity, TComponentData>(8, label);
        }

        public void Dispose()
        {
            m_HashMap.Dispose();
        }

        public bool TryGetValue(Entity entity, out TComponentData component)
        {
            if (!m_HashMap.TryGetValue(entity, out component))
            {
                if (!m_EntityComponentStore->HasComponent(entity, m_TypeIndex))
                {
                    return false;
                }
                component = *(TComponentData*) m_EntityComponentStore->GetComponentDataWithTypeRO(entity, m_TypeIndex);
                m_HashMap.TryAdd(entity, component);
            }

            return true;
        }
    }
}
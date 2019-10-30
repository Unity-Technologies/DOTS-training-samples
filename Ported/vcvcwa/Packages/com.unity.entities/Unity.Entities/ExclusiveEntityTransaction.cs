using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    [NativeContainer]
    public unsafe struct ExclusiveEntityTransaction
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        AtomicSafetyHandle m_Safety;
#endif

        internal EntityDataAccess EntityDataAccess => m_EntityDataAccess;
        internal EntityComponentStore* EntityComponentStore => m_EntityDataAccess.EntityComponentStore;
        internal ManagedComponentStore ManagedComponentStore => m_EntityDataAccess.ManagedComponentStore;
        internal EntityQueryManager EntityQueryManager => m_EntityDataAccess.EntityQueryManager;

        EntityDataAccess m_EntityDataAccess;

        internal ExclusiveEntityTransaction(EntityManager entityManager)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = new AtomicSafetyHandle();
#endif
            m_EntityDataAccess = new EntityDataAccess(entityManager, false);
        }

        internal void OnDestroy()
        {
            m_EntityDataAccess.Dispose();
        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal void SetAtomicSafetyHandle(AtomicSafetyHandle safety)
        {
            m_Safety = safety;
        }
#endif

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void CheckAccess()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
        }

        internal EntityArchetype CreateArchetype(ComponentType* types, int count)
        {
            CheckAccess();
            return m_EntityDataAccess.CreateArchetype(types, count);
        }

        public EntityArchetype CreateArchetype(params ComponentType[] types)
        {
            fixed (ComponentType* typesPtr = types)
            {
                return CreateArchetype(typesPtr, types.Length);
            }
        }

        public Entity CreateEntity(EntityArchetype archetype)
        {
            CheckAccess();
            return m_EntityDataAccess.CreateEntity(archetype);
        }

        public void CreateEntity(EntityArchetype archetype, NativeArray<Entity> entities)
        {
            CheckAccess();
            m_EntityDataAccess.CreateEntity(archetype, entities);
        }

        public Entity CreateEntity(params ComponentType[] types)
        {
            return CreateEntity(CreateArchetype(types));
        }

        public Entity Instantiate(Entity srcEntity)
        {
            Entity entity;
            InstantiateInternal(srcEntity, &entity, 1);
            return entity;
        }

        public void Instantiate(Entity srcEntity, NativeArray<Entity> outputEntities)
        {
            InstantiateInternal(srcEntity, (Entity*) outputEntities.GetUnsafePtr(), outputEntities.Length);
        }

        void InstantiateInternal(Entity srcEntity, Entity* outputEntities, int count)
        {
            CheckAccess();
            m_EntityDataAccess.InstantiateInternal(srcEntity, outputEntities, count);
        }

        public void DestroyEntity(NativeArray<Entity> entities)
        {
            DestroyEntityInternal((Entity*) entities.GetUnsafeReadOnlyPtr(), entities.Length);
        }

        public void DestroyEntity(NativeSlice<Entity> entities)
        {
            DestroyEntityInternal((Entity*) entities.GetUnsafeReadOnlyPtr(), entities.Length);
        }

        public void DestroyEntity(Entity entity)
        {
            DestroyEntityInternal(&entity, 1);
        }

        private void DestroyEntityInternal(Entity* entities, int count)
        {
            CheckAccess();
            m_EntityDataAccess.DestroyEntityInternal(entities, count);
        }

        void SetComponentDataRaw(Entity entity, int typeIndex, void* data, int size)
        {
            CheckAccess();
            m_EntityDataAccess.SetComponentDataRaw(entity, typeIndex, data, size);
        }

        public void AddComponent(Entity entity, ComponentType componentType)
        {
            CheckAccess();
            m_EntityDataAccess.AddComponent(entity, componentType);
        }

        public DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            CheckAccess();
            m_EntityDataAccess.AddComponent(entity, ComponentType.ReadWrite<T>());
            return GetBuffer<T>(entity);
        }

        public void RemoveComponent(Entity entity, ComponentType type)
        {
            CheckAccess();
            m_EntityDataAccess.RemoveComponent(entity, type);
        }

        public bool Exists(Entity entity)
        {
            CheckAccess();
            return m_EntityDataAccess.Exists(entity);
        }

        void SetSharedComponentDataBoxedDefaultMustBeNull(Entity entity, int typeIndex, int hashCode, object componentData)
        {
            CheckAccess();
            m_EntityDataAccess.SetSharedComponentDataBoxedDefaultMustBeNull(entity, typeIndex, hashCode, componentData);
        }

        void AddSharedComponentDataBoxedDefaultMustBeNull(Entity entity, int typeIndex, int hashCode, object componentData)
        {
            CheckAccess();
            m_EntityDataAccess.AddSharedComponentDataBoxedDefaultMustBeNull(entity, typeIndex, hashCode, componentData);
        }

        void SetComponentObject(Entity entity, ComponentType componentType, object componentObject)
        {
            CheckAccess();
            m_EntityDataAccess.SetComponentObject(entity, componentType, componentObject);
        }

        void SetBufferRaw(Entity entity, int componentTypeIndex, BufferHeader* tempBuffer, int sizeInChunk)
        {
            CheckAccess();
            m_EntityDataAccess.SetBufferRaw(entity, componentTypeIndex, tempBuffer, sizeInChunk);
        }

        public bool HasComponent(Entity entity, ComponentType type)
        {
            CheckAccess();
            return m_EntityDataAccess.HasComponent(entity, type);
        }

        public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
        {
            CheckAccess();
            return m_EntityDataAccess.GetComponentData<T>(entity);
        }

        void* GetComponentDataRawRW(Entity entity, int typeIndex)
        {
            CheckAccess();
            return m_EntityDataAccess.GetComponentDataRawRW(entity, typeIndex);
        }

        public void SetComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData
        {
            CheckAccess();
            m_EntityDataAccess.SetComponentData(entity, componentData);
        }

        public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
        {
            CheckAccess();
            return m_EntityDataAccess.GetSharedComponentData<T>(entity);
        }

        internal object GetSharedComponentData(Entity entity, int typeIndex)
        {
            CheckAccess();

            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            var sharedComponentIndex = EntityComponentStore->GetSharedComponentDataIndex(entity, typeIndex);
            return ManagedComponentStore.GetSharedComponentDataBoxed(sharedComponentIndex, typeIndex);
        }

        public void SetSharedComponentData<T>(Entity entity, T componentData) where T : struct, ISharedComponentData
        {
            CheckAccess();
            m_EntityDataAccess.SetSharedComponentData(entity, componentData);
        }

        internal void AddSharedComponent<T>(NativeArray<ArchetypeChunk> chunks, T componentData)
            where T : struct, ISharedComponentData
        {
            CheckAccess();
            var componentType = ComponentType.ReadWrite<T>();
            int sharedComponentIndex = ManagedComponentStore.InsertSharedComponent(componentData);
            m_EntityDataAccess.AddSharedComponentData(chunks, sharedComponentIndex, componentType);
            ManagedComponentStore.RemoveReference(sharedComponentIndex);
        }

        public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            CheckAccess();
            return m_EntityDataAccess.GetBuffer<T>(entity
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                ,m_Safety, m_Safety
#endif
                );
        }

        internal void AllocateConsecutiveEntitiesForLoading(int count)
        {
            CheckAccess();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (EntityComponentStore->CountEntities() != 0)
                throw new ArgumentException("loading into non-empty entity manager is not supported");
#endif       
            EntityComponentStore->AllocateConsecutiveEntitiesForLoading(count);
        }

        public void SwapComponents(ArchetypeChunk leftChunk, int leftIndex, ArchetypeChunk rightChunk, int rightIndex)
        {
            CheckAccess();
            m_EntityDataAccess.SwapComponents(leftChunk, leftIndex, rightChunk, rightIndex);
        }
    }
}
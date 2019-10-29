using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
    [NativeContainer]
    public unsafe struct ExclusiveEntityTransaction
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;
#endif
        [NativeDisableUnsafePtrRestriction] private GCHandle m_EntityQueryManager;

        [NativeDisableUnsafePtrRestriction] private GCHandle m_ManagedComponentStore;

        [NativeDisableUnsafePtrRestriction] internal EntityComponentStore* EntityComponentStore;

        internal ManagedComponentStore ManagedComponentStore => (ManagedComponentStore) m_ManagedComponentStore.Target;
        internal EntityQueryManager EntityQueryManager => (EntityQueryManager) m_EntityQueryManager.Target;

        internal ExclusiveEntityTransaction(EntityQueryManager entityQueryManager,
            ManagedComponentStore managedComponentStore, EntityComponentStore* componentStore)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = new AtomicSafetyHandle();
#endif
            EntityComponentStore = componentStore;
            m_EntityQueryManager = GCHandle.Alloc(entityQueryManager, GCHandleType.Weak);
            m_ManagedComponentStore = GCHandle.Alloc(managedComponentStore, GCHandleType.Weak);
        }

        internal void OnDestroy()
        {
            m_EntityQueryManager.Free();
            m_ManagedComponentStore.Free();
            EntityComponentStore = null;
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

            ComponentTypeInArchetype* typesInArchetype = stackalloc ComponentTypeInArchetype[count + 1];
            var componentCount = EntityManager.FillSortedArchetypeArray(typesInArchetype, types, count);

            EntityArchetype type;
            type.Archetype = EntityComponentStore->GetOrCreateArchetype(typesInArchetype, componentCount);

            return type;
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

            Entity entity;
            EntityComponentStore->CreateEntities(archetype.Archetype, &entity, 1);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
            return entity;
        }

        public void CreateEntity(EntityArchetype archetype, NativeArray<Entity> entities)
        {
            CheckAccess();
            EntityComponentStore->CreateEntities(archetype.Archetype, (Entity*) entities.GetUnsafePtr(),
                entities.Length);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
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

        private void InstantiateInternal(Entity srcEntity, Entity* outputEntities, int count)
        {
            CheckAccess();
            EntityComponentStore->InstantiateEntities(srcEntity, outputEntities, count);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
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
            EntityComponentStore->AssertCanDestroy(entities, count);
            EntityComponentStore->DestroyEntities(entities, count);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        public void AddComponent(Entity entity, ComponentType componentType)
        {
            CheckAccess();
            EntityComponentStore->AssertCanAddComponent(entity, componentType);
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            EntityComponentStore->AddComponent(entity, componentType);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        public DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            AddComponent(entity, ComponentType.ReadWrite<T>());
            return GetBuffer<T>(entity);
        }

        public void RemoveComponent(Entity entity, ComponentType type)
        {
            CheckAccess();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            EntityComponentStore->RemoveComponent(entity, type);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        public bool Exists(Entity entity)
        {
            CheckAccess();

            return EntityComponentStore->Exists(entity);
        }

        public bool HasComponent(Entity entity, ComponentType type)
        {
            CheckAccess();

            return EntityComponentStore->HasComponent(entity, type);
        }

        public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
        {
            CheckAccess();

            var typeIndex = TypeManager.GetTypeIndex<T>();
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            var ptr = EntityComponentStore->GetComponentDataWithTypeRO(entity, typeIndex);

            T data;
            UnsafeUtility.CopyPtrToStructure(ptr, out data);
            return data;
        }

        internal void* GetComponentDataRawRW(Entity entity, int typeIndex)
        {
            CheckAccess();

            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            return EntityComponentStore->GetComponentDataWithTypeRW(entity, typeIndex,
                EntityComponentStore->GlobalSystemVersion);
        }

        public void SetComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData
        {
            CheckAccess();

            var typeIndex = TypeManager.GetTypeIndex<T>();
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            var ptr = EntityComponentStore->GetComponentDataWithTypeRW(entity, typeIndex,
                EntityComponentStore->GlobalSystemVersion);
            UnsafeUtility.CopyStructureToPtr(ref componentData, ptr);
        }

        public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            var sharedComponentIndex = EntityComponentStore->GetSharedComponentDataIndex(entity, typeIndex);
            return ManagedComponentStore.GetSharedComponentData<T>(sharedComponentIndex);
        }

        internal object GetSharedComponentData(Entity entity, int typeIndex)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            var sharedComponentIndex = EntityComponentStore->GetSharedComponentDataIndex(entity, typeIndex);
            return ManagedComponentStore.GetSharedComponentDataBoxed(sharedComponentIndex, typeIndex);
        }

        public void SetSharedComponentData<T>(Entity entity, T componentData) where T : struct, ISharedComponentData
        {
            CheckAccess();

            var typeIndex = TypeManager.GetTypeIndex<T>();
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            var newSharedComponentDataIndex = ManagedComponentStore.InsertSharedComponent(componentData);

            EntityComponentStore->SetSharedComponentDataIndex(entity, typeIndex, newSharedComponentDataIndex);

            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
            ManagedComponentStore.RemoveReference(newSharedComponentDataIndex);
        }

        internal void AddSharedComponent<T>(NativeArray<ArchetypeChunk> chunks, T componentData)
            where T : struct, ISharedComponentData
        {
            CheckAccess();

            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            var componentType = ComponentType.ReadWrite<T>();
            var newSharedComponentDataIndex = ManagedComponentStore.InsertSharedComponent(componentData);
            EntityComponentStore->AssertCanAddComponent(chunks, componentType);

            EntityComponentStore->AddSharedComponent(chunks, componentType, newSharedComponentDataIndex);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
            ManagedComponentStore.RemoveReference(newSharedComponentDataIndex);
        }

        public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            CheckAccess();

            var typeIndex = TypeManager.GetTypeIndex<T>();

            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!TypeManager.IsBuffer(typeIndex))
                throw new ArgumentException(
                    $"GetBuffer<{typeof(T)}> may not be IComponentData or ISharedComponentData; currently {TypeManager.GetTypeInfo<T>().Category}");
#endif

            BufferHeader* header =
                (BufferHeader*) EntityComponentStore->GetComponentDataWithTypeRW(entity, typeIndex,
                    EntityComponentStore->GlobalSystemVersion);

            int internalCapacity = TypeManager.GetTypeInfo(typeIndex).BufferCapacity;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new DynamicBuffer<T>(header, m_Safety, m_Safety, false, internalCapacity);
#else
            return new DynamicBuffer<T>(header, internalCapacity);
#endif
        }

        internal void AllocateConsecutiveEntitiesForLoading(int count)
        {
            EntityComponentStore->AllocateConsecutiveEntitiesForLoading(count);
        }

        public void SwapComponents(ArchetypeChunk leftChunk, int leftIndex, ArchetypeChunk rightChunk, int rightIndex)
        {
            CheckAccess();
            var globalVersion = EntityComponentStore->GlobalSystemVersion;
            ChunkDataUtility.SwapComponents(leftChunk.m_Chunk, leftIndex, rightChunk.m_Chunk, rightIndex, 1,
                globalVersion, globalVersion);
        }
    }
}
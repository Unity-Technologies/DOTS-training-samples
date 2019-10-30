using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
    public sealed unsafe partial class EntityManager
    {
        // ----------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the number of shared components managed by this EntityManager.
        /// </summary>
        /// <returns>The shared component count</returns>
        public int GetSharedComponentCount()
        {
            return m_ManagedComponentStore.GetSharedComponentCount();
        }

        /// <summary>
        /// Gets the value of a component for an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <returns>A struct of type T containing the component value.</returns>
        /// <exception cref="ArgumentException">Thrown if the component type has no fields.</exception>
        public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
        {
            return m_EntityDataAccess.GetComponentData<T>(entity);
        }

        /// <summary>
        /// Sets the value of a component of an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="componentData">The data to set.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the component type has no fields.</exception>
        public void SetComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData
        {
            m_EntityDataAccess.SetComponentData(entity, componentData);
        }

        /// <summary>
        /// Gets the value of a chunk component.
        /// </summary>
        /// <remarks>
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        /// </remarks>
        /// <param name="chunk">The chunk.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A struct of type T containing the component value.</returns>
        /// <exception cref="ArgumentException">Thrown if the ArchetypeChunk object is invalid.</exception>
        public T GetChunkComponentData<T>(ArchetypeChunk chunk) where T : struct, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (chunk.Invalid())
                throw new System.ArgumentException(
                    $"GetChunkComponentData<{typeof(T)}> can not be called with an invalid archetype chunk.");
#endif
            var metaChunkEntity = chunk.m_Chunk->metaChunkEntity;
            return GetComponentData<T>(metaChunkEntity);
        }

        /// <summary>
        /// Gets the value of chunk component for the chunk containing the specified entity.
        /// </summary>
        /// <remarks>
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A struct of type T containing the component value.</returns>
        public T GetChunkComponentData<T>(Entity entity) where T : struct, IComponentData
        {
            EntityComponentStore->AssertEntitiesExist(&entity, 1);
            var chunk = EntityComponentStore->GetChunk(entity);
            var metaChunkEntity = chunk->metaChunkEntity;
            return GetComponentData<T>(metaChunkEntity);
        }

        /// <summary>
        /// Sets the value of a chunk component.
        /// </summary>
        /// <remarks>
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        /// </remarks>
        /// <param name="chunk">The chunk to modify.</param>
        /// <param name="componentValue">The component data to set.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the ArchetypeChunk object is invalid.</exception>
        public void SetChunkComponentData<T>(ArchetypeChunk chunk, T componentValue) where T : struct, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (chunk.Invalid())
                throw new System.ArgumentException(
                    $"SetChunkComponentData<{typeof(T)}> can not be called with an invalid archetype chunk.");
#endif
            var metaChunkEntity = chunk.m_Chunk->metaChunkEntity;
            SetComponentData<T>(metaChunkEntity, componentValue);
        }

        /// <summary>
        /// Gets the managed [UnityEngine.Component](https://docs.unity3d.com/ScriptReference/Component.html) object
        /// from an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of the managed object.</typeparam>
        /// <returns>The managed object, cast to type T.</returns>
        public T GetComponentObject<T>(Entity entity)
        {
            var componentType = ComponentType.ReadWrite<T>();
            return GetComponentObject<T>(entity, componentType);
        }
        
        public T GetComponentObject<T>(Entity entity, ComponentType componentType)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, componentType.TypeIndex);

            var entityInChunk = EntityComponentStore->GetEntityInChunk(entity);
            return (T) ManagedComponentStore.GetManagedObject(entityInChunk.Chunk, componentType, entityInChunk.IndexInChunk);
        }

        /// <summary>
        /// Sets the shared component of an entity.
        /// </summary>
        /// <remarks>
        /// Changing a shared component value of an entity results in the entity being moved to a
        /// different chunk. The entity moves to a chunk with other entities that have the same shared component values.
        /// A new chunk is created if no chunk with the same archetype and shared component values currently exists.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before setting the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity</param>
        /// <param name="componentData">A shared component object containing the values to set.</param>
        /// <typeparam name="T">The shared component type.</typeparam>
        public void SetSharedComponentData<T>(Entity entity, T componentData) where T : struct, ISharedComponentData
        {
            m_EntityDataAccess.SetSharedComponentData(entity, componentData);
        }

        /// <summary>
        /// Sets the shared component of all entities in the query.
        /// </summary>
        /// <remarks>
        /// The component data stays in the same chunk, the internal shared component data indices will be adjusted.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before setting the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity</param>
        /// <param name="componentData">A shared component object containing the values to set.</param>
        /// <typeparam name="T">The shared component type.</typeparam>
        public void SetSharedComponentData<T>(EntityQuery query, T componentData) where T : struct, ISharedComponentData
        {
            using (var chunks = query.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                if (chunks.Length == 0)
                    return;

                BeforeStructuralChange();

                var type = ComponentType.ReadWrite<T>();
                m_EntityDataAccess.RemoveComponent(chunks, type);

                int sharedComponentIndex = ManagedComponentStore.InsertSharedComponent(componentData);
                m_EntityDataAccess.AddSharedComponentData(chunks, sharedComponentIndex, type);
                ManagedComponentStore.RemoveReference(sharedComponentIndex);
            }
        }
        
        /// <summary>
        /// Gets a shared component from an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of shared component.</typeparam>
        /// <returns>A copy of the shared component.</returns>
        public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
        {
            return m_EntityDataAccess.GetSharedComponentData<T>(entity);
        }

        public int GetSharedComponentDataIndex<T>(Entity entity) where T : struct, ISharedComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            return EntityComponentStore->GetSharedComponentDataIndex(entity, typeIndex);
        }

        /// <summary>
        /// Gets a shared component by index.
        /// </summary>
        /// <remarks>
        /// The ECS framework maintains an internal list of unique shared components. You can get the components in this
        /// list, along with their indices using
        /// <see cref="GetAllUniqueSharedComponentData{T}(List{T},List{int})"/>. An
        /// index in the list is valid and points to the same shared component index as long as the shared component
        /// order version from <see cref="GetSharedComponentOrderVersion{T}(T)"/> remains the same.
        /// </remarks>
        /// <param name="sharedComponentIndex">The index of the shared component in the internal shared component
        /// list.</param>
        /// <typeparam name="T">The data type of the shared component.</typeparam>
        /// <returns>A copy of the shared component.</returns>
        public T GetSharedComponentData<T>(int sharedComponentIndex) where T : struct, ISharedComponentData
        {
            return m_ManagedComponentStore.GetSharedComponentData<T>(sharedComponentIndex);
        }
        
        /// <summary>
        /// Gets a list of all the unique instances of a shared component type.
        /// </summary>
        /// <remarks>
        /// All entities with the same archetype and the same values for a shared component are stored in the same set
        /// of chunks. This function finds the unique shared components existing across chunks and archetype and
        /// fills a list with copies of those components.
        /// </remarks>
        /// <param name="sharedComponentValues">A List<T> object to receive the unique instances of the
        /// shared component of type T.</param>
        /// <typeparam name="T">The type of shared component.</typeparam>
        public void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues)
            where T : struct, ISharedComponentData
        {
            m_ManagedComponentStore.GetAllUniqueSharedComponents(sharedComponentValues);
        }

        /// <summary>
        /// Gets a list of all unique shared components of the same type and a corresponding list of indices into the
        /// internal shared component list.
        /// </summary>
        /// <remarks>
        /// All entities with the same archetype and the same values for a shared component are stored in the same set
        /// of chunks. This function finds the unique shared components existing across chunks and archetype and
        /// fills a list with copies of those components and fills in a separate list with the indices of those components
        /// in the internal shared component list. You can use the indices to ask the same shared components directly
        /// by calling <see cref="GetSharedComponentData{T}(int)"/>, passing in the index. An index remains valid until
        /// the shared component order version changes. Check this version using
        /// <see cref="GetSharedComponentOrderVersion{T}(T)"/>.
        /// </remarks>
        /// <param name="sharedComponentValues"></param>
        /// <param name="sharedComponentIndices"></param>
        /// <typeparam name="T"></typeparam>
        public void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues, List<int> sharedComponentIndices)
            where T : struct, ISharedComponentData
        {
            m_ManagedComponentStore.GetAllUniqueSharedComponents(sharedComponentValues, sharedComponentIndices);
        }

        /// <summary>
        /// Gets the dynamic buffer of an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of the buffer's elements.</typeparam>
        /// <returns>The DynamicBuffer object for accessing the buffer contents.</returns>
        /// <exception cref="ArgumentException">Thrown if T is an unsupported type.</exception>
        public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            return m_EntityDataAccess.GetBuffer<T>(entity
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                , ComponentJobSafetyManager->GetSafetyHandle(typeIndex, false),
                ComponentJobSafetyManager->GetBufferSafetyHandle(typeIndex)
#endif
            );
        }

        /// <summary>
        /// Swaps the components of two entities.
        /// </summary>
        /// <remarks>
        /// The entities must have the same components. However, this function can swap the components of entities in
        /// different worlds, so they do not need to have identical archetype instances.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before swapping the components and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="leftChunk">A chunk containing one of the entities to swap.</param>
        /// <param name="leftIndex">The index within the `leftChunk` of the entity and components to swap.</param>
        /// <param name="rightChunk">The chunk containing the other entity to swap. This chunk can be the same as
        /// the `leftChunk`. It also does not need to be in the same World as `leftChunk`.</param>
        /// <param name="rightIndex">The index within the `rightChunk`  of the entity and components to swap.</param>
        public void SwapComponents(ArchetypeChunk leftChunk, int leftIndex, ArchetypeChunk rightChunk, int rightIndex)
        {
            m_EntityDataAccess.SwapComponents(leftChunk, leftIndex, rightChunk, rightIndex);
        }

        /// <summary>
        /// Gets the chunk in which the specified entity is stored.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The chunk containing the entity.</returns>
        public ArchetypeChunk GetChunk(Entity entity)
        {
            var chunk = EntityComponentStore->GetChunk(entity);
            return new ArchetypeChunk(chunk, EntityComponentStore);
        }

        /// <summary>
        /// Gets the number of component types associated with an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The number of components.</returns>
        public int GetComponentCount(Entity entity)
        {
            EntityComponentStore->AssertEntitiesExist(&entity, 1);
            var archetype = EntityComponentStore->GetArchetype(entity);
            return archetype->TypesCount - 1;
        }

        // ----------------------------------------------------------------------------------------------------------
        // INTERNAL
        // ----------------------------------------------------------------------------------------------------------

        internal void SetSharedComponentDataBoxedDefaultMustBeNull(Entity entity, int typeIndex, object componentData)
        {
            var hashCode = 0;
            if (componentData != null)
                hashCode = TypeManager.GetHashCode(componentData, typeIndex);
            
            SetSharedComponentDataBoxedDefaultMustBeNull(entity, typeIndex, hashCode, componentData);
        }

        void SetSharedComponentDataBoxedDefaultMustBeNull(Entity entity, int typeIndex, int hashCode, object componentData)
        {
            m_EntityDataAccess.SetSharedComponentDataBoxedDefaultMustBeNull(entity, typeIndex, hashCode, componentData);
        }

        internal void SetComponentObject(Entity entity, ComponentType componentType, object componentObject)
        {
            m_EntityDataAccess.SetComponentObject(entity, componentType, componentObject);
        }

        internal ComponentDataFromEntity<T> GetComponentDataFromEntity<T>(bool isReadOnly = false)
            where T : struct, IComponentData
        {
            var typeIndex = TypeManager.GetTypeIndex<T>();
            return GetComponentDataFromEntity<T>(typeIndex, isReadOnly);
        }

        internal ComponentDataFromEntity<T> GetComponentDataFromEntity<T>(int typeIndex, bool isReadOnly)
            where T : struct, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new ComponentDataFromEntity<T>(typeIndex, EntityComponentStore,
                ComponentJobSafetyManager->GetSafetyHandle(typeIndex, isReadOnly));
#else
            return new ComponentDataFromEntity<T>(typeIndex, EntityComponentStore);
#endif
        }

        internal BufferFromEntity<T> GetBufferFromEntity<T>(bool isReadOnly = false)
            where T : struct, IBufferElementData
        {
            return GetBufferFromEntity<T>(TypeManager.GetTypeIndex<T>(), isReadOnly);
        }

        internal BufferFromEntity<T> GetBufferFromEntity<T>(int typeIndex, bool isReadOnly = false)
            where T : struct, IBufferElementData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new BufferFromEntity<T>(typeIndex, EntityComponentStore, isReadOnly,
                ComponentJobSafetyManager->GetSafetyHandle(typeIndex, isReadOnly),
                ComponentJobSafetyManager->GetBufferSafetyHandle(typeIndex));
#else
            return new BufferFromEntity<T>(typeIndex, EntityComponentStore, isReadOnly);
#endif
        }

        internal void SetComponentDataRaw(Entity entity, int typeIndex, void* data, int size)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);
            ComponentJobSafetyManager->CompleteReadAndWriteDependency(typeIndex);

            m_EntityDataAccess.SetComponentDataRawEntityHasComponent(entity, typeIndex, data, size);
        }

        internal void* GetComponentDataRawRW(Entity entity, int typeIndex)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);
            ComponentJobSafetyManager->CompleteReadAndWriteDependency(typeIndex);

            return m_EntityDataAccess.GetComponentDataRawRWEntityHasComponent(entity, typeIndex);
        }

        internal void* GetComponentDataRawRO(Entity entity, int typeIndex)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);
            ComponentJobSafetyManager->CompleteReadAndWriteDependency(typeIndex);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (TypeManager.GetTypeInfo(typeIndex).IsZeroSized)
                throw new System.ArgumentException(
                    $"GetComponentDataRawRO can not be called with a zero sized component.");
#endif


            var ptr = EntityComponentStore->GetComponentDataWithTypeRO(entity, typeIndex);
            return ptr;
        }

        internal object GetSharedComponentData(Entity entity, int typeIndex)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            var sharedComponentIndex = EntityComponentStore->GetSharedComponentDataIndex(entity, typeIndex);
            return ManagedComponentStore.GetSharedComponentDataBoxed(sharedComponentIndex, typeIndex);
        }

        internal void* GetBufferRawRW(Entity entity, int typeIndex)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            ComponentJobSafetyManager->CompleteReadAndWriteDependency(typeIndex);

            BufferHeader* header =
                (BufferHeader*) EntityComponentStore->GetComponentDataWithTypeRW(entity, typeIndex,
                    EntityComponentStore->GlobalSystemVersion);

            return BufferHeader.GetElementPointer(header);
        }

        internal void* GetBufferRawRO(Entity entity, int typeIndex)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            ComponentJobSafetyManager->CompleteReadAndWriteDependency(typeIndex);

            BufferHeader* header = (BufferHeader*) EntityComponentStore->GetComponentDataWithTypeRO(entity, typeIndex);

            return BufferHeader.GetElementPointer(header);
        }

        internal int GetBufferLength(Entity entity, int typeIndex)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, typeIndex);

            ComponentJobSafetyManager->CompleteReadAndWriteDependency(typeIndex);

            BufferHeader* header = (BufferHeader*) EntityComponentStore->GetComponentDataWithTypeRO(entity, typeIndex);

            return header->Length;
        }

        internal object GetManagedComponentDataAsObject(Entity entity, ComponentType componentType)
        {
            EntityComponentStore->AssertEntityHasComponent(entity, componentType.TypeIndex);

            var entityInChunk = EntityComponentStore->GetEntityInChunk(entity);
            return ManagedComponentStore.GetManagedObject(entityInChunk.Chunk, componentType, entityInChunk.IndexInChunk);
        }
    }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
    public static unsafe partial class EntityManagerManagedComponentExtensions
    {
        /// <summary>
        /// Gets the value of a component for an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <returns>A struct of type T containing the component value.</returns>
        /// <exception cref="ArgumentException">Thrown if the component type has no fields.</exception>
        public static T GetComponentData<T>(this EntityManager manager, Entity entity) where T : class, IComponentData
        {
            var componentType = ComponentType.ReadWrite<T>();

            return (T) manager.GetManagedComponentDataAsObject(entity, componentType);
        }

        /// <summary>
        /// Sets the value of a component of an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="componentData">The data to set.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the component type has no fields.</exception>
        public static void SetComponentData<T>(this EntityManager manager, Entity entity, T componentData) where T : class, IComponentData
        {
            var type = ComponentType.ReadWrite<T>();
            manager.SetComponentObject(entity, type, componentData);
        }

        /// <summary>
        /// Gets the value of a chunk component.
        /// </summary>
        /// <remarks>
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        /// </remarks>
        /// <param name="chunk">The chunk.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A struct of type T containing the component value.</returns>
        /// <exception cref="ArgumentException">Thrown if the ArchetypeChunk object is invalid.</exception>
        public static T GetChunkComponentData<T>(this EntityManager manager, ArchetypeChunk chunk) where T : class, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (chunk.Invalid())
                throw new System.ArgumentException(
                    $"GetChunkComponentData<{typeof(T)}> can not be called with an invalid archetype chunk.");
#endif
            var metaChunkEntity = chunk.m_Chunk->metaChunkEntity;
            return manager.GetComponentData<T>(metaChunkEntity);
        }

        /// <summary>
        /// Gets the value of chunk component for the chunk containing the specified entity.
        /// </summary>
        /// <remarks>
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A struct of type T containing the component value.</returns>
        public static T GetChunkComponentData<T>(this EntityManager manager, Entity entity) where T : class, IComponentData
        {
            manager.EntityComponentStore->AssertEntitiesExist(&entity, 1);
            var chunk = manager.EntityComponentStore->GetChunk(entity);
            var metaChunkEntity = chunk->metaChunkEntity;
            return manager.GetComponentData<T>(metaChunkEntity);
        }

        /// <summary>
        /// Sets the value of a chunk component.
        /// </summary>
        /// <remarks>
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        /// </remarks>
        /// <param name="chunk">The chunk to modify.</param>
        /// <param name="componentValue">The component data to set.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <exception cref="ArgumentException">Thrown if the ArchetypeChunk object is invalid.</exception>
        public static void SetChunkComponentData<T>(this EntityManager manager, ArchetypeChunk chunk, T componentValue) where T : class, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (chunk.Invalid())
                throw new System.ArgumentException(
                    $"SetChunkComponentData<{typeof(T)}> can not be called with an invalid archetype chunk.");
#endif
            var metaChunkEntity = chunk.m_Chunk->metaChunkEntity;
            manager.SetComponentData<T>(metaChunkEntity, componentValue);
        }
    }
#endif
}
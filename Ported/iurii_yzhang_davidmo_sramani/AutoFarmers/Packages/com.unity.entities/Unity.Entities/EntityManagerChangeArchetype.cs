using System;
using JetBrains.Annotations;
using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
    [BurstCompile]
    unsafe struct StructuralChange
    {
        public delegate void AddComponentEntitiesBatchDelegate(EntityComponentStore* entityComponentStore, UnsafeList* entityBatchList, int typeIndex);
        public delegate bool AddComponentEntityDelegate(EntityComponentStore* entityComponentStore, Entity* entity, int typeIndex);
        public delegate void AddComponentChunksDelegate(EntityComponentStore* entityComponentStore, ArchetypeChunk* chunks, int chunkCount, int typeIndex);
        public delegate bool RemoveComponentEntityDelegate(EntityComponentStore* entityComponentStore, Entity* entity, int typeIndex);
        public delegate void RemoveComponentEntitiesBatchDelegate(EntityComponentStore* entityComponentStore, UnsafeList* entityBatchList, int typeIndex);
        public delegate void RemoveComponentChunksDelegate(EntityComponentStore* entityComponentStore, ArchetypeChunk* chunks, int chunkCount, int typeIndex);
        public delegate void AddSharedComponentChunksDelegate(EntityComponentStore* entityComponentStore, ArchetypeChunk* chunks, int chunkCount, int componentTypeIndex, int sharedComponentIndex);
        public delegate void MoveEntityArchetypeDelegate(EntityComponentStore* entityComponentStore, Entity* entity, Archetype* dstArchetype);
        public delegate void SetChunkComponentDelegate(EntityComponentStore* entityComponentStore, ArchetypeChunk* chunks, int chunkCount, void* componentData, int componentTypeIndex ); 

        public static AddComponentEntitiesBatchDelegate AddComponentEntitiesBatch;
        public static AddComponentEntityDelegate AddComponentEntity;
        public static AddComponentChunksDelegate AddComponentChunks;
        public static RemoveComponentEntityDelegate RemoveComponentEntity;
        public static RemoveComponentEntitiesBatchDelegate RemoveComponentEntitiesBatch;
        public static RemoveComponentChunksDelegate RemoveComponentChunks;
        public static AddSharedComponentChunksDelegate AddSharedComponentChunks;
        public static MoveEntityArchetypeDelegate MoveEntityArchetype;
        public static SetChunkComponentDelegate SetChunkComponent;
        
        public static void Initialize()
        {
//#if UNITY_DOTSPLAYER

//@Todo: 24 seconds on first enter playmode is too high a cost to pay on iteration time...
//       Need improvements in burst to make this work
#if true
            AddComponentEntitiesBatch = AddComponentEntitiesBatchExecute;
            AddComponentEntity = AddComponentEntityExecute;
            AddComponentChunks = AddComponentChunksExecute;
            RemoveComponentEntity = RemoveComponentEntityExecute;
            RemoveComponentEntitiesBatch = RemoveComponentEntitiesBatchExecute;
            RemoveComponentChunks = RemoveComponentChunksExecute;
            AddSharedComponentChunks = AddSharedComponentChunksExecute;
            MoveEntityArchetype = MoveEntityArchetypeExecute;
            SetChunkComponent = SetChunkComponentExecute;
            
#else
            AddComponentEntitiesBatch = BurstCompiler.CompileFunctionPointer<AddComponentEntitiesBatchDelegate>(AddComponentEntitiesBatchExecute).Invoke;
            AddComponentEntity = BurstCompiler.CompileFunctionPointer<AddComponentEntityDelegate>(AddComponentEntityExecute).Invoke;
            AddComponentChunks = BurstCompiler.CompileFunctionPointer<AddComponentChunksDelegate>(AddComponentChunksExecute).Invoke;
            RemoveComponentEntity = BurstCompiler.CompileFunctionPointer<RemoveComponentEntityDelegate>(RemoveComponentEntityExecute).Invoke;
            RemoveComponentEntitiesBatch = BurstCompiler.CompileFunctionPointer<RemoveComponentEntitiesBatchDelegate>(RemoveComponentEntitiesBatchExecute).Invoke;
            RemoveComponentChunks = BurstCompiler.CompileFunctionPointer<RemoveComponentChunksDelegate>(RemoveComponentChunksExecute).Invoke;
            AddSharedComponentChunks = BurstCompiler.CompileFunctionPointer<AddSharedComponentChunksDelegate>(AddSharedComponentChunksExecute).Invoke;
            MoveEntityArchetype = BurstCompiler.CompileFunctionPointer<MoveEntityArchetypeDelegate>(MoveEntityArchetypeExecute).Invoke;
            SetChunkComponent = BurstCompiler.CompileFunctionPointer<SetChunkComponentDelegate>(SetChunkComponentExecute).Invoke;
#endif
        }

        [BurstCompile]
        static void AddComponentEntitiesBatchExecute(EntityComponentStore* entityComponentStore, UnsafeList* entityBatchList, int typeIndex)
        {
            entityComponentStore->AddComponent(entityBatchList, ComponentType.FromTypeIndex(typeIndex), 0);
        }

        [BurstCompile]
        static bool AddComponentEntityExecute(EntityComponentStore* entityComponentStore, Entity* entity, int typeIndex)
        {
            return entityComponentStore->AddComponent(*entity, ComponentType.FromTypeIndex(typeIndex));
        }

        [BurstCompile]
        static void AddComponentChunksExecute(EntityComponentStore* entityComponentStore, ArchetypeChunk* chunks, int chunkCount, int typeIndex)
        {
            entityComponentStore->AddComponent(chunks, chunkCount, ComponentType.FromTypeIndex(typeIndex));
        }

        [BurstCompile]
        static bool RemoveComponentEntityExecute(EntityComponentStore* entityComponentStore, Entity* entity, int typeIndex)
        {
            return entityComponentStore->RemoveComponent(*entity, ComponentType.FromTypeIndex(typeIndex));
        }

        [BurstCompile]
        static void RemoveComponentEntitiesBatchExecute(EntityComponentStore* entityComponentStore, UnsafeList* entityBatchList, int typeIndex)
        {
            entityComponentStore->RemoveComponent(entityBatchList, ComponentType.FromTypeIndex(typeIndex));
        }

        [BurstCompile]
        static void RemoveComponentChunksExecute(EntityComponentStore* entityComponentStore, ArchetypeChunk* chunks, int chunkCount, int typeIndex)
        {
            entityComponentStore->RemoveComponent(chunks, chunkCount, ComponentType.FromTypeIndex(typeIndex));
        }

        [BurstCompile]
        static void AddSharedComponentChunksExecute(EntityComponentStore* entityComponentStore, ArchetypeChunk* chunks, int chunkCount, int componentTypeIndex, int sharedComponentIndex)
        {
            entityComponentStore->AddComponent(chunks, chunkCount, ComponentType.FromTypeIndex(componentTypeIndex), sharedComponentIndex);
        }

        [BurstCompile]
        static void MoveEntityArchetypeExecute(EntityComponentStore* entityComponentStore, Entity* entity, Archetype* dstArchetype)
        {
            entityComponentStore->Move(*entity, dstArchetype);
        }
        
        [BurstCompile]
        static void SetChunkComponentExecute(EntityComponentStore* entityComponentStore, ArchetypeChunk* chunks, int chunkCount, void* componentData, int componentTypeIndex)
        {
            entityComponentStore->SetChunkComponent(chunks, chunkCount, componentData, componentTypeIndex);
        }
    }

    public sealed unsafe partial class EntityManager
    {
        // ----------------------------------------------------------------------------------------------------------
        // PUBLIC
        // ----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Adds a component to an entity.
        /// </summary>
        /// <remarks>
        /// Adding a component changes the entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// The added component has the default values for the type.
        ///
        /// If the <see cref="Entity"/> object refers to an entity that has been destroyed, this function throws an ArgumentError
        /// exception.
        ///
        /// If the <see cref="Entity"/> object refers to an entity that already has the specified <see cref="ComponentType"/>,
        /// the function returns false without performing any modifications.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The Entity object.</param>
        /// <param name="componentType">The type of component to add.</param>
        public bool AddComponent(Entity entity, ComponentType componentType)
        {
            return m_EntityDataAccess.AddComponent(entity, componentType);
        }

        /// <summary>
        /// Adds a component to an entity.
        /// </summary>
        /// <remarks>
        /// Adding a component changes the entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// The added component has the default values for the type.
        ///
        /// If the <see cref="Entity"/> object refers to an entity that has been destroyed, this function throws an ArgumentError
        /// exception.
        ///
        /// If the <see cref="Entity"/> object refers to an entity that already has the specified <see cref="ComponentType"/>
        /// of type T, the function returns false without performing any modifications.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The Entity object.</param>
        /// <typeparam name="T">The type of component to add.</typeparam>
        public bool AddComponent<T>(Entity entity)
        {
            return AddComponent(entity, ComponentType.ReadWrite<T>());
        }

        /// <summary>
        /// Adds a component to a set of entities defined by a EntityQuery.
        /// </summary>
        /// <remarks>
        /// Adding a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// The added components have the default values for the type.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
        /// <param name="componentType">The type of component to add.</param>
        public void AddComponent(EntityQuery entityQuery, ComponentType componentType)
        {
            if (entityQuery.IsEmptyIgnoreFilter)
                return;
            
            m_EntityDataAccess.AddComponent(entityQuery.m_QueryData->MatchingArchetypes, entityQuery.m_Filter, componentType);
        }
        
        /// <summary>
        /// Adds a component to a set of entities defines by the EntityQuery and
        /// sets the component of each entity in the query to the value in the component array.
        /// componentArray.Length must match entityQuery.ToEntityArray().Length.
        /// </summary>
        /// <param name="entityQuery">THe EntityQuery defining the entities to add component to</param>
        /// <param name="componentArray"></param>
        public void AddComponentData<T>(EntityQuery entityQuery, NativeArray<T> componentArray) where T : struct, IComponentData
        {
            if (entityQuery.IsEmptyIgnoreFilter)
                return;

            using (var entities = entityQuery.ToEntityArray(Allocator.TempJob))
            {
                if (entities.Length != componentArray.Length)
                    throw new ArgumentException($"AddComponentData number of entities in query '{entities.Length}' must match componentArray.Length '{componentArray.Length}'.");

                AddComponent(entityQuery, ComponentType.ReadWrite<T>());

                var componentData = GetComponentDataFromEntity<T>();
                for (int i = 0; i != componentArray.Length; i++)
                    componentData[entities[i]] = componentArray[i];
            }
        }
        
        /// <summary>
        /// Adds a component to a set of entities defined by a EntityQuery.
        /// </summary>
        /// <remarks>
        /// Adding a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// The added components have the default values for the type.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
        /// <typeparam name="T">The type of component to add.</typeparam>
        public void AddComponent<T>(EntityQuery entityQuery)
        {
            AddComponent(entityQuery, ComponentType.ReadWrite<T>());
        }
        
        /// <summary>
        /// Adds a component to a set of entities.
        /// </summary>
        /// <remarks>
        /// Adding a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// The added components have the default values for the type.
        ///
        /// If an <see cref="Entity"/> object in the `entities` array refers to an entity that has been destroyed, this function
        /// throws an ArgumentError exception.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before creating these chunks and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entities">An array of Entity objects.</param>
        /// <param name="componentType">The type of component to add.</param>
        public void AddComponent(NativeArray<Entity> entities, ComponentType componentType)
        {
            if (componentType.IsChunkComponent)
                throw new ArgumentException($"Cannot add ChunkComponent {componentType.ToString()} on NativeArray of entities.");
                
            if (entities.Length == 0)
                return;

            for (int i = 0; i < entities.Length; i++)
            {
                EntityComponentStore->AssertCanAddComponent(entities[i], componentType);
            }

            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            if (entities.Length <= 10)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    StructuralChange.AddComponentEntity(EntityComponentStore, &entity, componentType.TypeIndex);
                }
            }
            else
            {
                NativeList<EntityBatchInChunk> entityBatchList;
                var batchesValid = m_EntityComponentStore->CreateEntityBatchListForAddComponent(entities, componentType, out entityBatchList);
                if (!batchesValid)
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        var entity = entities[i];
                        StructuralChange.AddComponentEntity(EntityComponentStore, &entity, componentType.TypeIndex);
                    }

                }
                else
                {
                    StructuralChange.AddComponentEntitiesBatch(EntityComponentStore, (UnsafeList*)NativeListUnsafeUtility.GetInternalListDataPtrUnchecked(ref entityBatchList), componentType.TypeIndex);
                    entityBatchList.Dispose();
                }
            }
            
            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        /// <summary>
        /// Remove a component from a set of entities.
        /// </summary>
        /// <remarks>
        /// Removing a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// If an <see cref="Entity"/> object in the `entities` array refers to an entity that has been destroyed, this function
        /// throws an ArgumentError exception.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before creating these chunks and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entities">An array of Entity objects.</param>
        /// <param name="componentType">The type of component to remove.</param>
        public void RemoveComponent(NativeArray<Entity> entities, ComponentType componentType)
        {
            if (entities.Length == 0)
                return;
            
            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();
            
            // For few entities, do on main thread.
            if (entities.Length <= 10)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    var entity = entities[i];
                    EntityComponentStore->AssertCanRemoveComponent(entity, componentType);
                    StructuralChange.RemoveComponentEntity(EntityComponentStore, &entity, componentType.TypeIndex);
                }
            }
            // For many entities, resort data into batches.
            else
            {
                NativeList<EntityBatchInChunk> entityBatchList;
                var batchesValid = m_EntityComponentStore->CreateEntityBatchListForRemoveComponent(entities, componentType, out entityBatchList);
                if (!batchesValid)
                {
                    for (int i = 0; i < entities.Length; i++)
                    {
                        var entity = entities[i];
                        EntityComponentStore->AssertCanRemoveComponent(entity, componentType);
                        StructuralChange.RemoveComponentEntity(EntityComponentStore, &entity, componentType.TypeIndex);
                    }
                }
                else
                {
                    StructuralChange.RemoveComponentEntitiesBatch(EntityComponentStore, (UnsafeList*)NativeListUnsafeUtility.GetInternalListDataPtrUnchecked(ref entityBatchList), componentType.TypeIndex);
                    entityBatchList.Dispose();
                }
            }
            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        /// <summary>
        /// Adds a component to a set of entities.
        /// </summary>
        /// <remarks>
        /// Adding a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// The added components have the default values for the type.
        ///
        /// If an <see cref="Entity"/> object in the `entities` array refers to an entity that has been destroyed, this function
        /// throws an ArgumentError exception.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before creating these chunks and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entities">An array of Entity objects.</param>
        /// <typeparam name="T">The type of component to add.</typeparam>
        public void AddComponent<T>(NativeArray<Entity> entities)
        {
            AddComponent(entities, ComponentType.ReadWrite<T>());
        }

        /// <summary>
        /// Adds a set of component to an entity.
        /// </summary>
        /// <remarks>
        /// Adding components changes the entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// The added components have the default values for the type.
        ///
        /// If the <see cref="Entity"/> object refers to an entity that has been destroyed, this function throws an ArgumentError
        /// exception.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding these components and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity to modify.</param>
        /// <param name="types">The types of components to add.</param>
        public void AddComponents(Entity entity, ComponentTypes types)
        {
            EntityComponentStore->AssertCanAddComponents(entity, types);
            
            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();
            
            EntityComponentStore->AddComponents(entity, types);
            
            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        /// <summary>
        /// Removes a component from an entity. Returns false if the entity did not have the component.
        /// </summary>
        /// <remarks>
        /// Removing a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before removing the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity to modify.</param>
        /// <param name="componentType">The type of component to remove.</param>
        public bool RemoveComponent(Entity entity, ComponentType componentType)
        {
            return m_EntityDataAccess.RemoveComponent(entity, componentType);
        }

        /// <summary>
        /// Removes a component from a set of entities defined by a EntityQuery.
        /// </summary>
        /// <remarks>
        /// Removing a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before removing the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
        /// <param name="componentType">The type of component to remove.</param>
        public void RemoveComponent(EntityQuery entityQuery, ComponentType componentType)
        {
            if (entityQuery.IsEmptyIgnoreFilter)
                return;

            RemoveComponent(entityQuery.m_QueryData->MatchingArchetypes, entityQuery.m_Filter, componentType);
        }

        /// <summary>
        /// Removes a set of components from a set of entities defined by a EntityQuery.
        /// </summary>
        /// <remarks>
        /// Removing a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before removing the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
        /// <param name="types">The types of components to add.</param>
        public void RemoveComponent(EntityQuery entityQuery, ComponentTypes types)
        {
            if (entityQuery.IsEmptyIgnoreFilter)
                return;

            if (entityQuery.CalculateEntityCount() == 0)
                return;

            // @TODO: Opportunity to do all components in batch on a per chunk basis.
            for (int i = 0; i != types.Length; i++)
                RemoveComponent(entityQuery, types.GetComponentType(i));
        }

        /// <summary>
        /// Removes a component from an entity. Returns false if the entity did not have the component.
        /// </summary>
        /// <remarks>
        /// Removing a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before removing the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        public bool RemoveComponent<T>(Entity entity)
        {
            return RemoveComponent(entity, ComponentType.ReadWrite<T>());
        }

        /// <summary>
        /// Removes a component from a set of entities defined by a EntityQuery.
        /// </summary>
        /// <remarks>
        /// Removing a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before removing the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        public void RemoveComponent<T>(EntityQuery entityQuery)
        {
            RemoveComponent(entityQuery, ComponentType.ReadWrite<T>());
        }

        /// <summary>
        /// Removes a component from a set of entities.
        /// </summary>
        /// <remarks>
        /// Removing a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before removing the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entities">An array identifying the entities to modify.</param>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        public void RemoveComponent<T>(NativeArray<Entity> entities)
        {
            RemoveComponent(entities, ComponentType.ReadWrite<T>());
        }


        /// <summary>
        /// Adds a component to an entity and set the value of that component. Returns true if the component was added,
        /// false if the entity already had the component. (The component's data is set either way.)
        /// </summary>
        /// <remarks>
        /// Adding a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk. 
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <param name="componentData">The data to set.</param>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns></returns>
        public bool AddComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData
        {
            var type = ComponentType.ReadWrite<T>();
            var added = AddComponent(entity, type);
            if (!type.IsZeroSized)
                SetComponentData(entity, componentData);
            
            return added;
        }

        /// <summary>
        /// Removes a chunk component from the specified entity. Returns false if the entity did not have the component.
        /// </summary>
        /// <remarks>
        /// A chunk component is common to all entities in a chunk. Removing the chunk component from an entity changes
        /// that entity's archetype and results in the entity being moved to a different chunk (that does not have the
        /// removed component).
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before removing the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        public bool RemoveChunkComponent<T>(Entity entity)
        {
            return RemoveComponent(entity, ComponentType.ChunkComponent<T>());
        }

        /// <summary>
        /// Adds a chunk component to the specified entity. Returns true if the chunk component was added, false if the
        /// entity already had the chunk component. (The chunk component's data is set either way.)
        /// </summary>
        /// <remarks>
        /// Adding a chunk component to an entity changes that entity's archetype and results in the entity being moved
        /// to a different chunk, either one that already has an archetype containing the chunk component or a new
        /// chunk.
        ///
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk. In either case, getting
        /// or setting the component reads or writes the same data.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of component, which must implement IComponentData.</typeparam>
        public bool AddChunkComponentData<T>(Entity entity) where T : struct, IComponentData
        {
            return AddComponent(entity, ComponentType.ChunkComponent<T>()); 
        }

        /// <summary>
        /// Adds a component to each of the chunks identified by a EntityQuery and set the component values.
        /// </summary>
        /// <remarks>
        /// This function finds all chunks whose archetype satisfies the EntityQuery and adds the specified
        /// component to them.
        ///
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery identifying the chunks to modify.</param>
        /// <param name="componentData">The data to set.</param>
        /// <typeparam name="T">The type of component, which must implement IComponentData.</typeparam>
        public void AddChunkComponentData<T>(EntityQuery entityQuery, T componentData) where T : unmanaged, IComponentData 
        {
            if (entityQuery.IsEmptyIgnoreFilter)
                return;

            using (var chunks = entityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                if (chunks.Length == 0)
                    return;
                
                EntityComponentStore->AssertCanAddChunkComponent(chunks, ComponentType.ChunkComponent<T>());
                
                BeforeStructuralChange();
                var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();
                
                var componentType = ComponentType.ReadWrite<T>();
                var componentTypeIndex = componentType.TypeIndex;
                var componentTypeIndexForAdd = TypeManager.MakeChunkComponentTypeIndex(componentTypeIndex);
                ArchetypeChunk* chunkPtr = (ArchetypeChunk*)NativeArrayUnsafeUtility.GetUnsafePtr(chunks);

                StructuralChange.AddComponentChunks(EntityComponentStore, chunkPtr, chunks.Length, componentTypeIndexForAdd);
                StructuralChange.SetChunkComponent(EntityComponentStore, chunkPtr, chunks.Length, &componentData, componentTypeIndex);

                var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
                EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
                ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
            }
        }

        /// <summary>
        /// Removes a component from the chunks identified by a EntityQuery.
        /// </summary>
        /// <remarks>
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before removing the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery identifying the chunks to modify.</param>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        public void RemoveChunkComponentData<T>(EntityQuery entityQuery)
        {
            RemoveComponent(entityQuery, ComponentType.ChunkComponent<T>());
        }

        /// <summary>
        /// Adds a dynamic buffer component to an entity.
        /// </summary>
        /// <remarks>
        /// A buffer component stores the number of elements inside the chunk defined by the [InternalBufferCapacity]
        /// attribute applied to the buffer element type declaration. Any additional elements are stored in a separate memory
        /// block that is managed by the EntityManager.
        ///
        /// Adding a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the buffer and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of buffer element. Must implement IBufferElementData.</typeparam>
        /// <returns>The buffer.</returns>
        /// <seealso cref="InternalBufferCapacityAttribute"/>
        public DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData
        {
            AddComponent(entity, ComponentType.ReadWrite<T>());
            return GetBuffer<T>(entity);
        }

        /// <summary>
        /// Adds a managed [UnityEngine.Component](https://docs.unity3d.com/ScriptReference/Component.html)
        /// object to an entity.
        /// </summary>
        /// <remarks>
        /// Accessing data in a managed object forfeits many opportunities for increased performance. Adding
        /// managed objects to an entity should be avoided or used sparingly.
        ///
        /// Adding a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the object and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity to modify.</param>
        /// <param name="componentData">An object inheriting UnityEngine.Component.</param>
        /// <exception cref="ArgumentNullException">If the componentData object is not an instance of
        /// UnityEngine.Component.</exception>
        public void AddComponentObject(Entity entity, object componentData)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (componentData == null)
                throw new ArgumentNullException(nameof(componentData));
#endif

            ComponentType type = componentData.GetType();

            AddComponent(entity, type);
            SetComponentObject(entity, type, componentData);
        }

        /// <summary>
        /// Adds a shared component to an entity. Returns true if the shared component was added, false if the entity
        /// already had the shared component. (The shared component's data is set either way.)
        /// </summary>
        /// <remarks>
        /// The fields of the `componentData` parameter are assigned to the added shared component.
        ///
        /// Adding a component to an entity changes its archetype and results in the entity being moved to a
        /// different chunk. The entity moves to a chunk with other entities that have the same shared component values.
        /// A new chunk is created if no chunk with the same archetype and shared component values currently exists.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <param name="componentData">An instance of the shared component having the values to set.</param>
        /// <typeparam name="T">The shared component type.</typeparam>
        public bool AddSharedComponentData<T>(Entity entity, T componentData) where T : struct, ISharedComponentData
        {
            return m_EntityDataAccess.AddSharedComponentData(entity, componentData);
        }

        /// <summary>
        /// Adds a shared component to a set of entities defined by a EntityQuery.
        /// </summary>
        /// <remarks>
        /// The fields of the `componentData` parameter are assigned to all of the added shared components.
        ///
        /// Adding a component to an entity changes its archetype and results in the entity being moved to a
        /// different chunk. The entity moves to a chunk with other entities that have the same shared component values.
        /// A new chunk is created if no chunk with the same archetype and shared component values currently exists.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery defining a set of entities to modify.</param>
        /// <param name="componentData">The data to set.</param>
        /// <typeparam name="T">The data type of the shared component.</typeparam>
        public void AddSharedComponentData<T>(EntityQuery entityQuery, T componentData)
            where T : struct, ISharedComponentData
        {
            if (entityQuery.IsEmptyIgnoreFilter)
                return;

            var componentType = ComponentType.ReadWrite<T>();
            using (var chunks = entityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                if (chunks.Length == 0)
                    return;
                var newSharedComponentDataIndex = ManagedComponentStore.InsertSharedComponent(componentData);
                m_EntityDataAccess.AddSharedComponentData(chunks, newSharedComponentDataIndex, componentType);
                m_ManagedComponentStore.RemoveReference(newSharedComponentDataIndex);
            }
        }

        public void SetArchetype(Entity entity, EntityArchetype archetype)
        {
            EntityComponentStore->AssertEntitiesExist(&entity, 1);
            
            var oldArchetype = EntityComponentStore->GetArchetype(entity);
            var newArchetype = archetype.Archetype;
            
            Unity.Entities.EntityComponentStore.AssertArchetypeDoesNotRemoveSystemStateComponents(oldArchetype, newArchetype);
            
            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();
            
            StructuralChange.MoveEntityArchetype(EntityComponentStore, &entity, archetype.Archetype);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        /// <summary>
        /// Enabled entities are processed by systems, disabled entities are not.
        /// Adds or removes the <see cref="Disabled"/> component. By default EntityQuery does not include entities containing the Disabled component.
        ///
        /// If the entity was converted from a prefab and thus has a <see cref="LinkedEntityGroup"/> component, the entire group will enabled or disabled.
        /// </summary>
        /// <param name="entity">The entity to enable or disable</param>
        /// <param name="enabled">True if the entity should be enabled</param>
        public void SetEnabled(Entity entity, bool enabled)
        {
            if (GetEnabled(entity) == enabled)
                return;

            var disabledType = ComponentType.ReadWrite<Disabled>();
            if (HasComponent<LinkedEntityGroup>(entity))
            {
                //@TODO: AddComponent / Remove component should support Allocator.Temp
                using (var linkedEntities = GetBuffer<LinkedEntityGroup>(entity).Reinterpret<Entity>().ToNativeArray(Allocator.TempJob))
                {
                    if (enabled)
                        RemoveComponent(linkedEntities, disabledType);
                    else
                        AddComponent(linkedEntities, disabledType);
                }
            }
            else
            {
                if (!enabled)
                    AddComponent(entity, disabledType);
                else
                    RemoveComponent(entity, disabledType);
            }
        }

        public bool GetEnabled(Entity entity)
        {
            return !HasComponent<Disabled>(entity);
        }

        // ----------------------------------------------------------------------------------------------------------
        // INTERNAL
        // ----------------------------------------------------------------------------------------------------------

        void RemoveComponent(UnsafeMatchingArchetypePtrList archetypeList, EntityQueryFilter filter, ComponentType componentType)
        {
            m_EntityDataAccess.RemoveComponent(archetypeList, filter, componentType);
        }
        
        // these are used by tiny, do not remove
        [UsedImplicitly]
        internal void AddComponentRaw(Entity entity, int typeIndex) => AddComponent(entity, ComponentType.FromTypeIndex(typeIndex));
        [UsedImplicitly]
        internal void RemoveComponentRaw(Entity entity, int typeIndex) => RemoveComponent(entity, ComponentType.FromTypeIndex(typeIndex));
    }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
    public static unsafe partial class EntityManagerManagedComponentExtensions
    {
        /// <summary>
        /// Adds a component to an entity and set the value of that component.
        /// </summary>
        /// <remarks>
        /// Adding a component changes an entity's archetype and results in the entity being moved to a different
        /// chunk.
        /// 
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <param name="componentData">The data to set.</param>
        /// <typeparam name="T">The type of component.</typeparam>
        public static void AddComponentData<T>(this EntityManager manager, Entity entity, T componentData) where T : class, IComponentData
        {
            var type = ComponentType.ReadWrite<T>();

            manager.AddComponent(entity, type);
            manager.SetComponentData(entity, componentData);
        }

        /// <summary>
        /// Adds a chunk component to the specified entity.
        /// </summary>
        /// <remarks>
        /// Adding a chunk component to an entity changes that entity's archetype and results in the entity being moved
        /// to a different chunk, either one that already has an archetype containing the chunk component or a new
        /// chunk.
        ///
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk. In either case, getting
        /// or setting the component reads or writes the same data.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <typeparam name="T">The type of component, which must implement IComponentData.</typeparam>
        public static void AddChunkComponentData<T>(this EntityManager manager, Entity entity) where T : class, IComponentData
        {
            manager.AddComponent(entity, ComponentType.ChunkComponent<T>());
        }

        /// <summary>
        /// Adds a component to each of the chunks identified by a EntityQuery and set the component values.
        /// </summary>
        /// <remarks>
        /// This function finds all chunks whose archetype satisfies the EntityQuery and adds the specified
        /// component to them.
        ///
        /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
        /// instance through either the chunk itself or through an entity stored in that chunk.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding the component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entityQuery">The EntityQuery identifying the chunks to modify.</param>
        /// <param name="componentData">The data to set.</param>
        /// <typeparam name="T">The type of component, which must implement IComponentData.</typeparam>
        public static void AddChunkComponentData<T>(this EntityManager manager, EntityQuery entityQuery, T componentData) where T : class, IComponentData
        {
            using (var chunks = entityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                if (chunks.Length == 0)
                    return;
                
                manager.EntityComponentStore->AssertCanAddChunkComponent(chunks, ComponentType.ChunkComponent<T>());
                
                manager.BeforeStructuralChange();
                var archetypeChanges = manager.EntityComponentStore->BeginArchetypeChangeTracking();

                var type = ComponentType.ReadWrite<T>();
                var chunkType = ComponentType.FromTypeIndex(TypeManager.MakeChunkComponentTypeIndex(type.TypeIndex));
                
                StructuralChange.AddComponentChunks(manager.EntityComponentStore, (ArchetypeChunk*)NativeArrayUnsafeUtility.GetUnsafePtr(chunks), chunks.Length, chunkType.TypeIndex);

                var changedArchetypes = manager.EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
                manager.EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
                manager.ManagedComponentStore.Playback(ref manager.EntityComponentStore->ManagedChangesTracker);
                
                manager.SetChunkComponent(chunks, componentData);
            }
        }

        static void SetChunkComponent<T>(this EntityManager manager, NativeArray<ArchetypeChunk> chunks, T componentData) where T : class, IComponentData
        {
            var type = TypeManager.MakeChunkComponentTypeIndex(TypeManager.GetTypeIndex<T>());

            for (int i = 0; i < chunks.Length; i++)
            {
                var srcChunk = chunks[i].m_Chunk;
                var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(srcChunk->Archetype, type);

                var entityInChunk = manager.EntityComponentStore->GetEntityInChunk(srcChunk->metaChunkEntity);
                manager.ManagedComponentStore.SetManagedObject(entityInChunk.Chunk, indexInTypeArray, entityInChunk.IndexInChunk, componentData);
            }
        }
    }
#endif
}

using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities
{
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
        /// If the <see cref="Entity"/> object refers to an entity that already has the specified <see cref="ComponentType">,
        /// the function returns returns without performing any modifications.
        ///
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding thes component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The Entity object.</param>
        /// <param name="componentType">The type of component to add.</param>
        public void AddComponent(Entity entity, ComponentType componentType)
        {
            AddComponent(entity, componentType, true);
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
        /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
        /// currently running Jobs to complete before adding thes component and no additional Jobs can start before
        /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
        /// be able to make use of the processing power of all available cores.
        /// </remarks>
        /// <param name="entity">The Entity object.</param>
        /// <typeparam name="T">The type of component to add.</typeparam>
        public void AddComponent<T>(Entity entity)
        {
            AddComponent(entity, ComponentType.ReadWrite<T>());
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
            var iterator = entityQuery.GetComponentChunkIterator();
            AddComponent(iterator.m_MatchingArchetypeList, iterator.m_Filter, componentType);
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
            using (var entities = entityQuery.ToEntityArray(Allocator.TempJob))
            {
                if (entities.Length != componentArray.Length)
                    throw new System.ArgumentException($"AddComponentData number of entities in query '{entities.Length}' must match componentArray.Length '{componentArray.Length}'.");

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
            if (entities.Length == 0)
                return;

            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            using (var entityBatchList = m_EntityComponentStore->CreateEntityBatchList(entities))
            {
                EntityComponentStore->AddComponentFromMainThread(entityBatchList, componentType, 0);
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

            using (var entityBatchList = m_EntityComponentStore->CreateEntityBatchList(entities))
            {
                EntityComponentStore->RemoveComponentFromMainThread(entityBatchList, componentType, 0);
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
            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            EntityComponentStore->AssertCanAddComponents(entity, types);
            EntityComponentStore->AddComponents(entity, types);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        /// <summary>
        /// Removes a component from an entity.
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
        /// <param name="type">The type of component to remove.</param>
        public void RemoveComponent(Entity entity, ComponentType type)
        {
            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            EntityComponentStore->RemoveComponent(entity, type);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
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
            var iterator = entityQuery.GetComponentChunkIterator();
            RemoveComponent(iterator.m_MatchingArchetypeList, iterator.m_Filter, componentType);
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
            if (entityQuery.CalculateEntityCount() == 0)
                return;

            // @TODO: Opportunity to do all components in batch on a per chunk basis.
            for (int i = 0; i != types.Length; i++)
                RemoveComponent(entityQuery, types.GetComponentType(i));
        }

        /// <summary>
        /// Removes a component from an entity.
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
        public void RemoveComponent<T>(Entity entity)
        {
            RemoveComponent(entity, ComponentType.ReadWrite<T>());
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
        public void AddComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData
        {
            var type = ComponentType.ReadWrite<T>();
            AddComponent(entity, type, type.IgnoreDuplicateAdd);
            if (!type.IsZeroSized)
                SetComponentData(entity, componentData);
        }

        /// <summary>
        /// Removes a chunk component from the specified entity.
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
        public void RemoveChunkComponent<T>(Entity entity)
        {
            RemoveComponent(entity, ComponentType.ChunkComponent<T>());
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
        public void AddChunkComponentData<T>(Entity entity) where T : struct, IComponentData
        {
            AddComponent(entity, ComponentType.ChunkComponent<T>());
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
        public void AddChunkComponentData<T>(EntityQuery entityQuery, T componentData) where T : struct, IComponentData
        {
            using (var chunks = entityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                if (chunks.Length == 0)
                    return;
                BeforeStructuralChange();
                var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

                EntityComponentStore->AssertCanAddChunkComponent(chunks, ComponentType.ChunkComponent<T>());

                var type = ComponentType.ReadWrite<T>();
                var chunkType = ComponentType.FromTypeIndex(TypeManager.MakeChunkComponentTypeIndex(type.TypeIndex));

                using (var entityBatchList = EntityComponentStore->CreateEntityBatchList(chunks))
                {
                    EntityComponentStore->AddComponentFromMainThread(entityBatchList, chunkType, 0);
                    EntityComponentStore->SetChunkComponent<T>(entityBatchList, componentData);
                }

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
        /// Adds a shared component to an entity.
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
        public void AddSharedComponentData<T>(Entity entity, T componentData) where T : struct, ISharedComponentData
        {
            //TODO: optimize this (no need to move the entity to a new chunk twice)
            AddComponent(entity, ComponentType.ReadWrite<T>(), false);
            SetSharedComponentData(entity, componentData);
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
            var componentType = ComponentType.ReadWrite<T>();
            using (var chunks = entityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                var newSharedComponentDataIndex = m_ManagedComponentStore.InsertSharedComponent(componentData);
                AddSharedComponentData(chunks, newSharedComponentDataIndex, componentType);
            }
        }

        public void SetArchetype(Entity entity, EntityArchetype archetype)
        {
            BeforeStructuralChange();

            EntityComponentStore->AssertEntitiesExist(&entity, 1);
            var oldArchetype = EntityComponentStore->GetArchetype(entity);
            var newArchetype = archetype.Archetype;
            Unity.Entities.EntityComponentStore.AssertArchetypeDoesNotRemoveSystemStateComponents(oldArchetype, newArchetype);

            var archetypeChanges =  EntityComponentStore->BeginArchetypeChangeTracking();
            EntityComponentStore->SetArchetype(entity, archetype);
            var changedArchetypes =  EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);

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

        internal void AddComponent(Entity entity, ComponentType componentType, bool ignoreDuplicateAdd)
        {
            if (!Exists(entity))
                return;

            if (ignoreDuplicateAdd && HasComponent(entity, componentType))
                return;


            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            EntityComponentStore->AssertCanAddComponent(entity, componentType);
            EntityComponentStore->AddComponent(entity, componentType);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        internal void AddComponent(UnsafeMatchingArchetypePtrList archetypeList, EntityQueryFilter filter, ComponentType componentType)
        {
            var jobHandle = new JobHandle();
            //@TODO: Missing EntityQuery.SyncFilter
            using (var chunks = ComponentChunkIterator.CreateArchetypeChunkArray(archetypeList, Allocator.TempJob, out jobHandle, ref filter))
            {
                jobHandle.Complete();
                if (chunks.Length == 0)
                    return;

                BeforeStructuralChange();
                var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

                EntityComponentStore->AssertCanAddComponent(chunks, componentType);

                using (var entityBatchList = m_EntityComponentStore->CreateEntityBatchList(chunks))
                {
                    EntityComponentStore->AddComponentFromMainThread(entityBatchList, componentType, 0);
                }

                var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
                EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
                ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
            }
        }

        internal void AddSharedComponentDataBoxedDefaultMustBeNull(Entity entity, int typeIndex, int hashCode, object componentData)
        {
            //TODO: optimize this (no need to move the entity to a new chunk twice)
            AddComponent(entity, ComponentType.FromTypeIndex(typeIndex));
            SetSharedComponentDataBoxedDefaultMustBeNull(entity, typeIndex, hashCode, componentData);
        }

        internal void AddSharedComponentDataBoxedDefaultMustBeNull(UnsafeMatchingArchetypePtrList archetypeList, EntityQueryFilter filter, int typeIndex, int hashCode, object componentData)
        {
            var newSharedComponentDataIndex = 0;
            if (componentData != null) // null means default
                newSharedComponentDataIndex = ManagedComponentStore.InsertSharedComponentAssumeNonDefault(typeIndex, hashCode, componentData);

            AddSharedComponentData(archetypeList, filter, newSharedComponentDataIndex, ComponentType.FromTypeIndex(typeIndex));
        }

        internal void AddSharedComponentData(NativeArray<ArchetypeChunk> chunks, int sharedComponentIndex, ComponentType componentType)
        {
            if (chunks.Length == 0)
                return;
            
            BeforeStructuralChange();
            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();

            EntityComponentStore->AssertCanAddComponent(chunks, componentType);
            EntityComponentStore->AddSharedComponent(chunks, componentType, sharedComponentIndex);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
            ManagedComponentStore.RemoveReference(sharedComponentIndex);
        }

        internal void AddSharedComponentData(UnsafeMatchingArchetypePtrList archetypeList, EntityQueryFilter filter,
            int sharedComponentIndex, ComponentType componentType)
        {
            var jobHandle = new JobHandle();
            //@TODO: Missing EntityQuery.SyncFilter
            using (var chunks = ComponentChunkIterator.CreateArchetypeChunkArray(archetypeList, Allocator.TempJob, out jobHandle, ref filter))
            {
                jobHandle.Complete();
                AddSharedComponentData(chunks, sharedComponentIndex, componentType);
            }
        }
        
        internal void RemoveComponent(NativeArray<ArchetypeChunk> chunks, ComponentType componentType)
        {
            if (chunks.Length == 0)
                return;

            BeforeStructuralChange();

            var archetypeChanges = EntityComponentStore->BeginArchetypeChangeTracking();
            EntityComponentStore->AssertCanRemoveComponent(chunks, componentType);

            EntityComponentStore->RemoveComponent(chunks, componentType);

            var changedArchetypes = EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);
            ManagedComponentStore.Playback(ref EntityComponentStore->ManagedChangesTracker);
        }

        internal void RemoveComponent(UnsafeMatchingArchetypePtrList archetypeList, EntityQueryFilter filter, ComponentType componentType)
        {
            //@TODO: Missing EntityQuery.SyncFilter
            var jobHandle = new JobHandle();
            using (var chunks = ComponentChunkIterator.CreateArchetypeChunkArray(archetypeList, Allocator.TempJob, out jobHandle, ref filter))
            {
                jobHandle.Complete();
                RemoveComponent(chunks, componentType);
            }
        }
        
        internal void AddComponentRaw(Entity entity, int typeIndex)
        {
            AddComponent(entity, ComponentType.FromTypeIndex(typeIndex));
        }

        internal void RemoveComponentRaw(Entity entity, int typeIndex)
        {
            RemoveComponent(entity, ComponentType.FromTypeIndex(typeIndex));
        }
    }
}

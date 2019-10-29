using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;

namespace Unity.Entities
{
    public class EntityQueryDescValidationException : Exception
    {
        public EntityQueryDescValidationException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// Defines a queryDesc to find archetypes with specific components.
    /// </summary>
    /// <remarks>
    /// A queryDesc combines components in the All, Any, and None sets according to the
    /// following rules:
    ///
    /// * All - Includes archetypes that have every component in this set
    /// * Any - Includes archetypes that have at least one component in this set
    /// * None - Excludes archetypes that have any component in this set
    ///
    /// For example, given entities with the following components:
    ///
    /// * Player has components: Position, Rotation, Player
    /// * Enemy1 has components: Position, Rotation, Melee
    /// * Enemy2 has components: Position, Rotation, Ranger
    ///
    /// The queryDesc below would give you all of the archetypes that:
    /// have any of [Melee or Ranger], AND have none of [Player], AND have all of [Position and Rotation]
    /// <code>
    /// new EntityQueryDesc {
    ///     Any = new ComponentType[] {typeof(Melee), typeof(Ranger)},
    ///     None = new ComponentType[] {typeof(Player)},
    ///     All = new ComponentType[] {typeof(Position), typeof(Rotation)}
    /// }
    /// </code>
    ///
    /// In other words, the queryDesc selects the Enemy1 and Enemy2 entities, but not the Player entity.
    /// </remarks>
    public class EntityQueryDesc
    {
        /// <summary>
        /// The queryDesc includes archetypes that contain at least one (but possibly more) of the
        /// components in the Any list.
        /// </summary>
        public ComponentType[] Any = Array.Empty<ComponentType>();
        /// <summary>
        /// The queryDesc excludes archetypes that contain any of the
        /// components in the None list.
        /// </summary>
        public ComponentType[] None = Array.Empty<ComponentType>();
        /// <summary>
        /// The queryDesc includes archetypes that contain all of the
        /// components in the All list.
        /// </summary>
        public ComponentType[] All = Array.Empty<ComponentType>();
        /// <summary>
        /// Specialized queryDesc options.
        /// </summary>
        /// <remarks>
        /// You should not need to set these options for most queriesDesc.
        ///
        /// Options is a bit mask; use the bitwise OR operator to combine multiple options.
        /// </remarks>
        public EntityQueryOptions Options = EntityQueryOptions.Default;

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void ValidateComponentTypes(ComponentType[] componentTypes, ref NativeArray<int> allComponentTypeIds, ref int curComponentTypeIndex)
        {
            for (int i = 0; i < componentTypes.Length; i++)
            {
                var componentType = componentTypes[i];
                allComponentTypeIds[curComponentTypeIndex++] = componentType.TypeIndex;
                if (componentType.AccessModeType == ComponentType.AccessMode.Exclude)
                    throw new ArgumentException("EntityQueryDesc cannot contain Exclude Component types");
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public void Validate()
        {
            // Determine the number of ComponentTypes contained in the filters
            var itemCount = None.Length + All.Length + Any.Length;

            // Project all the ComponentType Ids of None, All, Any queryDesc filters into the same array to identify duplicated later on
            // Also, check that queryDesc doesn't contain any ExcludeComponent...

            var allComponentTypeIds = new NativeArray<int>(itemCount, Allocator.Temp);
            var curComponentTypeIndex = 0;
            ValidateComponentTypes(None, ref allComponentTypeIds, ref curComponentTypeIndex);
            ValidateComponentTypes(All, ref allComponentTypeIds, ref curComponentTypeIndex);
            ValidateComponentTypes(Any, ref allComponentTypeIds, ref curComponentTypeIndex);

            // Check for duplicate, only if necessary
            if (itemCount > 1)
            {
                // Sort the Ids to have identical value adjacent
                allComponentTypeIds.Sort();

                // Check for identical values
                var refId = allComponentTypeIds[0];
                for (int i = 1; i < allComponentTypeIds.Length; i++)
                {
                    var curId = allComponentTypeIds[i];
                    if (curId == refId)
                    {
#if NET_DOTS
                        throw new EntityQueryDescValidationException(
                            $"EntityQuery contains a filter with duplicate component type index {curId}.  Queries can only contain a single component of a given type in a filter.");
#else
                        var compType = TypeManager.GetType(curId);
                        throw new EntityQueryDescValidationException(
                            $"EntityQuery contains a filter with duplicate component type name {compType.Name}.  Queries can only contain a single component of a given type in a filter.");
#endif
                    }

                    refId = curId;
                }
            }
        }
    }

    /// <summary>
    /// The bit flags to use for the <see cref="EntityQueryDesc.Options"/> field.
    /// </summary>
    [Flags]
    public enum EntityQueryOptions
    {
        /// <summary>
        /// No options specified.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The queryDesc includes the special <see cref="Prefab"/> component.
        /// </summary>
        IncludePrefab = 1,
        /// <summary>
        /// The queryDesc includes the special <see cref="Disabled"/> component.
        /// </summary>
        IncludeDisabled = 2,
        /// <summary>
        /// The queryDesc should filter selected entities based on the
        /// <see cref="WriteGroupAttribute"/> settings of the components specified in the queryDesc.
        /// </summary>
        FilterWriteGroup = 4,
    }

    /// <summary>
    /// This mask can be used to quickly identify if an entity would be returned by an EntityQuery.  There is a maximum
    /// limit of 1024 EntityQueryMasks that can be created.  EntityQueryMasks cannot be created from EntityQueries
    /// with filters.
    /// </summary>
    /// <seealso cref="EntityManager.GetEntityQueryMask"/>
    public unsafe struct EntityQueryMask
    {
        internal byte Index;
        internal byte Mask;

        [NativeDisableUnsafePtrRestriction]
        internal readonly EntityComponentStore* EntityComponentStore;

        internal EntityQueryMask(byte index, byte mask, EntityComponentStore* entityComponentStore)
        {
            Index = index;
            Mask = mask;
            EntityComponentStore = entityComponentStore;
        }

        internal bool IsCreated()
        {
            return EntityComponentStore != null;
        }

        /// <summary>
        /// Confirms whether an entity would belong to the EntityQuery used to create this EntityQueryMask.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>True if the entity would be returned by the EntityQuery, false if it would not.</returns>
        public bool Matches(Entity entity)
        {
            return EntityComponentStore->GetArchetype(entity)->CompareMask(this);
        }
    };

    /// <summary>
    /// A EntityQuery provides a queryDesc-based view of your component data.
    /// </summary>
    /// <remarks>
    /// A EntityQuery defines a view of your data based on a queryDesc for the set of
    /// component types that an archetype must contain in order for its chunks and entities
    /// to be included in the view. You can also exclude archetypes that contain specific types
    /// of components. For simple queriesDesc, you can create a EntityQuery based on an array of
    /// component types. The following example defines a EntityQuery that finds all entities
    /// with both RotationQuaternion and RotationSpeed components.
    ///
    /// <code>
    /// EntityQuery m_Group = GetEntityQuery(typeof(RotationQuaternion),
    ///                                            ComponentType.ReadOnly{RotationSpeed}());
    /// </code>
    ///
    /// The queryDesc uses `ComponentType.ReadOnly` instead of the simpler `typeof` expression
    /// to designate that the system does not write to RotationSpeed. Always specify read only
    /// when possible, since there are fewer constraints on read access to data, which can help
    /// the Job scheduler execute your Jobs more efficiently.
    ///
    /// For more complex queriesDesc, you can use an <see cref="EntityQueryDesc"/> instead of a
    /// simple list of component types.
    ///
    /// Use the <see cref="EntityManager.CreateEntityQuery"/> or
    /// <see cref="ComponentSystemBase.GetEntityQuery"/> functions
    /// to get a EntityQuery instance.
    /// </remarks>
    public unsafe class EntityQuery : IDisposable
    {
        readonly ComponentJobSafetyManager* m_SafetyManager;
        internal readonly EntityQueryData*  m_QueryData;
        readonly EntityComponentStore*      m_EntityComponentStore;
        internal EntityQueryFilter          m_Filter;
        private ManagedComponentStore       m_ManagedComponentStore;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal string                    DisallowDisposing = null;
#endif

        // TODO: this is temporary, used to cache some state to avoid recomputing the TransformAccessArray. We need to improve this.
        internal IDisposable               m_CachedState;

        internal EntityComponentStore* EntityComponentStore => m_EntityComponentStore;
        internal ManagedComponentStore ManagedComponentStore => m_ManagedComponentStore;
        internal ComponentJobSafetyManager* SafetyManager => m_SafetyManager;

        internal EntityQuery(EntityQueryData* queryData, ComponentJobSafetyManager* safetyManager, EntityComponentStore* entityComponentStore, ManagedComponentStore managedComponentStore)
        {
            m_QueryData = queryData;
            m_Filter = default(EntityQueryFilter);
            m_SafetyManager = safetyManager;
            m_EntityComponentStore = entityComponentStore;
            m_ManagedComponentStore = managedComponentStore;
        }

        /// <summary>
        ///      Ignore this EntityQuery if it has no entities in any of its archetypes.
        /// </summary>
        /// <returns>True if this EntityQuery has no entities. False if it has 1 or more entities.</returns>
        public bool IsEmptyIgnoreFilter
        {
            get
            {
                for (var m = 0; m < m_QueryData->MatchingArchetypes.Length; ++m)
                {
                    var match = m_QueryData->MatchingArchetypes.Ptr[m];
                    if (match->Archetype->EntityCount > 0)
                        return false;
                }

                return true;
            }
        }
#if NET_DOTS
        internal class SlowListSet<T>
        {
            internal List<T> items;

            internal SlowListSet() {
                items = new List<T>();
            }

            internal void Add(T item)
            {
                if (!items.Contains(item))
                    items.Add(item);
            }

            internal int Count => items.Count;

            internal T[] ToArray()
            {
                return items.ToArray();
            }
        }
#endif

        /// <summary>
        /// Gets the array of <see cref="ComponentType"/> objects included in this EntityQuery.
        /// </summary>
        /// <returns>Array of ComponentTypes</returns>
        internal ComponentType[] GetQueryTypes()
        {
#if !NET_DOTS
            var types = new HashSet<ComponentType>();
#else
            var types = new SlowListSet<ComponentType>();
#endif

            for (var i = 0; i < m_QueryData->ArchetypeQueryCount; ++i)
            {
                for (var j = 0; j < m_QueryData->ArchetypeQuery[i].AnyCount; ++j)
                {
                    types.Add(TypeManager.GetType(m_QueryData->ArchetypeQuery[i].Any[j]));
                }
                for (var j = 0; j < m_QueryData->ArchetypeQuery[i].AllCount; ++j)
                {
                    types.Add(TypeManager.GetType(m_QueryData->ArchetypeQuery[i].All[j]));
                }
                for (var j = 0; j < m_QueryData->ArchetypeQuery[i].NoneCount; ++j)
                {
                    types.Add(ComponentType.Exclude(TypeManager.GetType(m_QueryData->ArchetypeQuery[i].None[j])));
                }
            }

#if !NET_DOTS
            var array = new ComponentType[types.Count];
            var t = 0;
            foreach (var type in types)
                array[t++] = type;
            return array;
#else
            return types.ToArray();
#endif
        }

        /// <summary>
        ///     Packed array of this EntityQuery's ReadOnly and writable ComponentTypes.
        ///     ReadOnly ComponentTypes come before writable types in this array.
        /// </summary>
        /// <returns>Array of ComponentTypes</returns>
        internal ComponentType[] GetReadAndWriteTypes()
        {
            var types = new ComponentType[m_QueryData->ReaderTypesCount + m_QueryData->WriterTypesCount];
            var typeArrayIndex = 0;
            for (var i = 0; i < m_QueryData->ReaderTypesCount; ++i)
            {
                types[typeArrayIndex++] = ComponentType.ReadOnly(TypeManager.GetType(m_QueryData->ReaderTypes[i]));
            }
            for (var i = 0; i < m_QueryData->WriterTypesCount; ++i)
            {
                types[typeArrayIndex++] = TypeManager.GetType(m_QueryData->WriterTypes[i]);
            }

            return types;
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (DisallowDisposing != null)
                throw new ArgumentException(DisallowDisposing);
#endif

            if (m_CachedState != null)
                m_CachedState.Dispose();

            ResetFilter();
        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        /// <summary>
        ///     Gets safety handle to a ComponentType required by this EntityQuery.
        /// </summary>
        /// <param name="indexInEntityQuery">Index of a ComponentType in this EntityQuery's RequiredComponents list./param>
        /// <returns>AtomicSafetyHandle for a ComponentType</returns>
        internal AtomicSafetyHandle GetSafetyHandle(int indexInEntityQuery)
        {
            var type = m_QueryData->RequiredComponents + indexInEntityQuery;
            var isReadOnly = type->AccessModeType == ComponentType.AccessMode.ReadOnly;
            return m_SafetyManager->GetSafetyHandle(type->TypeIndex, isReadOnly);
        }

        /// <summary>
        ///     Gets buffer safety handle to a ComponentType required by this EntityQuery.
        /// </summary>
        /// <param name="indexInEntityQuery">Index of a ComponentType in this EntityQuery's RequiredComponents list./param>
        /// <returns>AtomicSafetyHandle for a buffer</returns>
        internal AtomicSafetyHandle GetBufferSafetyHandle(int indexInEntityQuery)
        {
            var type = m_QueryData->RequiredComponents + indexInEntityQuery;
            return m_SafetyManager->GetBufferSafetyHandle(type->TypeIndex);
        }
#endif

        bool GetIsReadOnly(int indexInEntityQuery)
        {
            var type = m_QueryData->RequiredComponents + indexInEntityQuery;
            var isReadOnly = type->AccessModeType == ComponentType.AccessMode.ReadOnly;
            return isReadOnly;
        }

        /// <summary>
        /// Calculates the number of entities selected by this EntityQuery.
        /// </summary>
        /// <remarks>
        /// The EntityQuery must run the queryDesc and apply any filters to calculate the entity count.
        /// </remarks>
        /// <returns>The number of entities based on the current EntityQuery properties.</returns>
        public int CalculateEntityCount()
        {
            SyncFilterTypes();

            // no filtering case does not benefit from Burst, so just do it normally on main thread
            if(!m_Filter.RequiresMatchesFilter)
                return ChunkIterationUtility.CalculateEntityCount(m_QueryData->MatchingArchetypes, ref m_Filter);
            
            var entityCount = stackalloc int[1];
            new CalculateEntityCountJob
            {
                MatchingArchetypes = m_QueryData->MatchingArchetypes,
                Filter = m_Filter,
                OutEntityCount = entityCount
            }.Run();

            return *entityCount;
        }

        /// <summary>
        /// Calculates the number of entities selected by this EntityQuery, ignoring any set filters.
        /// </summary>
        /// <remarks>
        /// The EntityQuery must run the queryDesc to calculate the entity count.
        /// </remarks>
        /// <returns>The number of entities based on the current EntityQuery properties.</returns>
        public int CalculateEntityCountWithoutFiltering()
        {
            // no filtering case does not benefit from Burst, so just do it normally on main thread
            var dummyFilter = default(EntityQueryFilter);
            return ChunkIterationUtility.CalculateEntityCount(m_QueryData->MatchingArchetypes, ref dummyFilter);
        }

        /// <summary>
        /// Calculates the number of chunks that match this EntityQuery.
        /// </summary>
        /// <remarks>
        /// The EntityQuery must run the queryDesc and apply any filters to calculate the chunk count.
        /// </remarks>
        /// <returns>The number of entities based on the current EntityQuery properties.</returns>
        public int CalculateChunkCount()
        {
            SyncFilterTypes();

            // no filtering case does not benefit from Burst, so just do it normally on main thread
            if(!m_Filter.RequiresMatchesFilter)
                return ChunkIterationUtility.CalculateChunkCount(m_QueryData->MatchingArchetypes, ref m_Filter);
            
            var chunkCount = stackalloc int[1];
            new CalculateChunkCountJob
            {
                MatchingArchetypes = m_QueryData->MatchingArchetypes,
                Filter = m_Filter,
                OutChunkCount = chunkCount
            }.Run();

            return *chunkCount;
        }

        /// <summary>
        /// Calculates the number of chunks that match this EntityQuery, ignoring any set filters.
        /// </summary>
        /// <remarks>
        /// The EntityQuery must run the queryDesc to calculate the chunk count.
        /// </remarks>
        /// <returns>The number of entities based on the current EntityQuery properties.</returns>
        public int CalculateChunkCountWithoutFiltering()
        {
            // no filtering case does not benefit from Burst, so just do it normally on main thread
            var dummyFilter = default(EntityQueryFilter);
            return ChunkIterationUtility.CalculateChunkCount(m_QueryData->MatchingArchetypes, ref dummyFilter);
        }

        /// <summary>
        /// Gets an ArchetypeChunkIterator which can be used to iterate over every chunk returned by this EntityQuery.
        /// </summary>
        /// <returns>ArchetypeChunkIterator for this EntityQuery</returns>
        public ArchetypeChunkIterator GetArchetypeChunkIterator()
        {
            return new ArchetypeChunkIterator(m_QueryData->MatchingArchetypes, m_SafetyManager, m_EntityComponentStore->GlobalSystemVersion, ref m_Filter);
        }

        /// <summary>
        ///     Index of a ComponentType in this EntityQuery's RequiredComponents list.
        ///     For example, you have a EntityQuery that requires these ComponentTypes: Position, Velocity, and Color.
        ///
        ///     These are their type indices (according to the TypeManager):
        ///         Position.TypeIndex == 3
        ///         Velocity.TypeIndex == 5
        ///            Color.TypeIndex == 17
        ///
        ///     RequiredComponents: [Position -> Velocity -> Color] (a linked list)
        ///     Given Velocity's TypeIndex (5), the return value would be 1, since Velocity is in slot 1 of RequiredComponents.
        /// </summary>
        /// <param name="componentType">Index of a ComponentType in the TypeManager</param>
        /// <returns>An index into RequiredComponents.</returns>
        internal int GetIndexInEntityQuery(int componentType)
        {
            // Go through all the required component types in this EntityQuery until you find the matching component type index.
            var componentIndex = 0;
            while (componentIndex < m_QueryData->RequiredComponentsCount && m_QueryData->RequiredComponents[componentIndex].TypeIndex != componentType)
                ++componentIndex;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (componentIndex >= m_QueryData->RequiredComponentsCount || m_QueryData->RequiredComponents[componentIndex].AccessModeType == ComponentType.AccessMode.Exclude)
                throw new InvalidOperationException( $"Trying to get iterator for {TypeManager.GetType(componentType)} but the required component type was not declared in the EntityQuery.");
#endif
            return componentIndex;
        }

        /// <summary>
        ///     Creates an array with all the chunks in this EntityQuery.
        ///     Gives the caller a job handle so it can wait for GatherChunks to finish.
        /// </summary>
        /// <param name="allocator">Allocator to use for the array.</param>
        /// <param name="jobhandle">Handle to the GatherChunks job used to fill the output array.</param>
        /// <returns>NativeArray of all the chunks in this ComponentChunkIterator.</returns>
        public NativeArray<ArchetypeChunk> CreateArchetypeChunkArray(Allocator allocator, out JobHandle jobhandle)
        {
            JobHandle dependency = default(JobHandle);

            var filterCount = m_Filter.Changed.Count;
            if (filterCount > 0)
            {
                var readerTypes = stackalloc int[filterCount];
                    for (int i = 0; i < filterCount; ++i)
                        readerTypes[i] = m_QueryData->RequiredComponents[m_Filter.Changed.IndexInEntityQuery[i]].TypeIndex;

                dependency = m_SafetyManager->GetDependency(readerTypes, filterCount,null, 0);
            }

            return ChunkIterationUtility.CreateArchetypeChunkArrayWithoutSync(m_QueryData->MatchingArchetypes, allocator, out jobhandle, ref m_Filter, dependency);
        }

        /// <summary>
        ///     Creates an array with all the chunks in this EntityQuery.
        ///     Waits for the GatherChunks job to complete here.
        /// </summary>
        /// <param name="allocator">Allocator to use for the array.</param>
        /// <returns>NativeArray of all the chunks in this ComponentChunkIterator.</returns>
        public NativeArray<ArchetypeChunk> CreateArchetypeChunkArray(Allocator allocator)
        {
            SyncFilterTypes();
            JobHandle job;
            var res = ChunkIterationUtility.CreateArchetypeChunkArrayWithoutSync(m_QueryData->MatchingArchetypes, allocator, out job, ref m_Filter);
            job.Complete();
            return res;
        }

        /// <summary>
        /// Creates a NativeArray containing the selected entities.
        /// </summary>
        /// <param name="allocator">The type of memory to allocate.</param>
        /// <param name="jobhandle">A handle that you can use as a dependency for a Job
        /// that uses the NativeArray.</param>
        /// <returns>An array containing all the entities selected by the EntityQuery.</returns>
        public NativeArray<Entity> ToEntityArray(Allocator allocator, out JobHandle jobhandle)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var entityType = new ArchetypeChunkEntityType(m_SafetyManager->GetEntityManagerSafetyHandle());
#else
            var entityType = new ArchetypeChunkEntityType();
#endif

            return ChunkIterationUtility.CreateEntityArray(m_QueryData->MatchingArchetypes, allocator, entityType,  this, ref m_Filter, out jobhandle, GetDependency());
        }

        /// <summary>
        /// Creates a NativeArray containing the selected entities.
        /// </summary>
        /// <remarks>This version of the function blocks until the Job used to fill the array is complete.</remarks>
        /// <param name="allocator">The type of memory to allocate.</param>
        /// <returns>An array containing all the entities selected by the EntityQuery.</returns>
        public NativeArray<Entity> ToEntityArray(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var entityType = new ArchetypeChunkEntityType(m_SafetyManager->GetEntityManagerSafetyHandle());
#else
            var entityType = new ArchetypeChunkEntityType();
#endif
            JobHandle job;
            var res = ChunkIterationUtility.CreateEntityArray(m_QueryData->MatchingArchetypes, allocator, entityType, this, ref m_Filter, out job, GetDependency());
            job.Complete();
            return res;
        }

        internal struct GatherEntitiesResult
        {
            public int StartingOffset;
            public int EntityCount;
            public Entity* EntityBuffer;
            public NativeArray<Entity> EntityArray;
        }

        internal void GatherEntitiesToArray(out GatherEntitiesResult result)
        {
            // The ChunkIterationUtility will perform the job if the query is not using filter
            ChunkIterationUtility.GatherEntitiesToArray(m_QueryData, ref m_Filter, out result);

            if (result.EntityBuffer == null)
            {
                var entityCount = CalculateEntityCount();
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                var entityType = new ArchetypeChunkEntityType(m_SafetyManager->GetEntityManagerSafetyHandle());
#else
                var entityType = new ArchetypeChunkEntityType();
#endif
                var job = new GatherEntitiesJob
                {
                    EntityType = entityType,
                    Entities = new NativeArray<Entity>(entityCount, Allocator.TempJob)
                };
                job.Run(this);
                result.EntityArray = job.Entities;
                result.EntityBuffer = (Entity*)result.EntityArray.GetUnsafeReadOnlyPtr();
                result.EntityCount = result.EntityArray.Length;
            }
        }

        internal void ReleaseGatheredEntities(ref GatherEntitiesResult result)
        {
            ChunkIterationUtility.currentOffsetInResultBuffer = result.StartingOffset;
            if (result.EntityArray.IsCreated)
            {
                result.EntityArray.Dispose();
            }
        }
        
        /// <summary>
        /// Creates a NativeArray containing the components of type T for the selected entities.
        /// </summary>
        /// <param name="allocator">The type of memory to allocate.</param>
        /// <param name="jobhandle">A handle that you can use as a dependency for a Job
        /// that uses the NativeArray.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>An array containing the specified component for all the entities selected
        /// by the EntityQuery.</returns>
        public NativeArray<T> ToComponentDataArray<T>(Allocator allocator, out JobHandle jobhandle)
            where T : struct, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var componentType = new ArchetypeChunkComponentType<T>(m_SafetyManager->GetSafetyHandle(TypeManager.GetTypeIndex<T>(), true), true, EntityComponentStore->GlobalSystemVersion);
#else
            var componentType = new ArchetypeChunkComponentType<T>(true, EntityComponentStore->GlobalSystemVersion);
#endif
            return ChunkIterationUtility.CreateComponentDataArray(m_QueryData->MatchingArchetypes, allocator, componentType, this, ref m_Filter, out jobhandle, GetDependency());
        }

        /// <summary>
        /// Creates a NativeArray containing the components of type T for the selected entities.
        /// </summary>
        /// <param name="allocator">The type of memory to allocate.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>An array containing the specified component for all the entities selected
        /// by the EntityQuery.</returns>
        /// <exception cref="InvalidOperationException">Thrown if you ask for a component that is not part of
        /// the group.</exception>
        public NativeArray<T> ToComponentDataArray<T>(Allocator allocator)
            where T : struct, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var componentType = new ArchetypeChunkComponentType<T>(m_SafetyManager->GetSafetyHandle(TypeManager.GetTypeIndex<T>(), true), true, EntityComponentStore->GlobalSystemVersion);
#else
            var componentType = new ArchetypeChunkComponentType<T>(true, EntityComponentStore->GlobalSystemVersion);
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            int typeIndex = TypeManager.GetTypeIndex<T>();
            int indexInEntityQuery = GetIndexInEntityQuery(typeIndex);
            if (indexInEntityQuery == -1)
                throw new InvalidOperationException( $"Trying ToComponentDataArray of {TypeManager.GetType(typeIndex)} but the required component type was not declared in the EntityGroup.");
#endif

            JobHandle job;
            var res = ChunkIterationUtility.CreateComponentDataArray(m_QueryData->MatchingArchetypes, allocator, componentType, this, ref m_Filter, out job, GetDependency());
            job.Complete();
            return res;
        }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
        public T[] ToComponentDataArray<T>() where T : class, IComponentData
        {
            int typeIndex = TypeManager.GetTypeIndex<T>();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            int indexInEntityQuery = GetIndexInEntityQuery(typeIndex);
            if (indexInEntityQuery == -1)
                throw new InvalidOperationException($"Trying ToComponentDataArray of {TypeManager.GetType(typeIndex)} but the required component type was not declared in the EntityGroup.");

            var componentType = new ArchetypeChunkComponentType<T>(m_SafetyManager->GetSafetyHandle(typeIndex, true), true, EntityComponentStore->GlobalSystemVersion);
#else
            var componentType = new ArchetypeChunkComponentType<T>(true, EntityComponentStore->GlobalSystemVersion);
#endif

            var matches = m_QueryData->MatchingArchetypes;
            var entityCount = ChunkIterationUtility.CalculateEntityCount(matches, ref m_Filter);
            T[] res = new T[entityCount];
            int i = 0;
            for (int mi = 0; mi < matches.Length; ++mi)
            {
                var match = m_QueryData->MatchingArchetypes.Ptr[mi];
                var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(match->Archetype, typeIndex);
                var chunks = match->Archetype->Chunks;

                for (int ci = 0; ci < chunks.Count; ++ci)
                {
                    var chunk = chunks.p[ci];
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    if (chunk->ManagedArrayIndex < 0)
                        throw new ArgumentException($"Trying ToComponentDataArray of managed component '{TypeManager.GetType(typeIndex)}' but the found chunk from the query doesn't contain any managed components.");
#endif
                    var objs = ManagedComponentStore.GetManagedObjectRange(chunk, indexInTypeArray, out var rangeStart, out var rangeLength);
                    int rangeEnd = rangeStart + rangeLength;
                    for (int oi = rangeStart; oi < rangeEnd; ++oi)
                    {
                        res[i++] = (T)objs[oi];
                    }
                }
            }

            return res;
        }
#endif

        public void CopyFromComponentDataArray<T>(NativeArray<T> componentDataArray)
        where T : struct, IComponentData
        {
            // throw if non equal size
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var entityCount = CalculateEntityCount();
            if (entityCount != componentDataArray.Length)
                throw new ArgumentException($"Length of input array ({componentDataArray.Length}) does not match length of EntityQuery ({entityCount})");
            var componentType = new ArchetypeChunkComponentType<T>(m_SafetyManager->GetSafetyHandle(TypeManager.GetTypeIndex<T>(), false), false, EntityComponentStore->GlobalSystemVersion);
#else
            var componentType = new ArchetypeChunkComponentType<T>(false, EntityComponentStore->GlobalSystemVersion);
#endif

            ChunkIterationUtility.CopyFromComponentDataArray(m_QueryData->MatchingArchetypes, componentDataArray, componentType, this, ref m_Filter, out var job, GetDependency());
            job.Complete();
        }

        public void CopyFromComponentDataArray<T>(NativeArray<T> componentDataArray, out JobHandle jobhandle)
            where T : struct,IComponentData
        {
            // throw if non equal size
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var entityCount = CalculateEntityCount();
            if(entityCount != componentDataArray.Length)
                throw new ArgumentException($"Length of input array ({componentDataArray.Length}) does not match length of EntityQuery ({entityCount})");
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var componentType = new ArchetypeChunkComponentType<T>(m_SafetyManager->GetSafetyHandle(TypeManager.GetTypeIndex<T>(), false), false, EntityComponentStore->GlobalSystemVersion);
#else
            var componentType = new ArchetypeChunkComponentType<T>(false, EntityComponentStore->GlobalSystemVersion);
#endif

            ChunkIterationUtility.CopyFromComponentDataArray(m_QueryData->MatchingArchetypes, componentDataArray, componentType, this, ref m_Filter, out jobhandle, GetDependency());
        }

        public Entity GetSingletonEntity()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var entityCount = CalculateEntityCount();
            if (entityCount != 1)
                throw new System.InvalidOperationException($"GetSingletonEntity() requires that exactly one exists but there are {entityCount}.");
#endif

            Entity entity;

            var iterator = GetArchetypeChunkIterator();
            iterator.MoveNext();

            var array = iterator.GetCurrentChunkComponentDataPtr(false, 0);
            UnsafeUtility.CopyPtrToStructure(array, out entity);
            
            return entity;
        }

        /// <summary>
        /// Gets the value of a singleton component.
        /// </summary>
        /// <remarks>A singleton component is a component of which only one instance exists in the world
        /// and which has been set with <see cref="SetSingleton{T}(T)"/>.</remarks>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A copy of the singleton component.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T GetSingleton<T>()
            where T : struct, IComponentData
        {
            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(GetIndexInEntityQuery(TypeManager.GetTypeIndex<T>()) != 1)
                throw new System.InvalidOperationException($"GetSingleton<{typeof(T)}>() requires that {typeof(T)} is the only component type in its archetype.");

            var entityCount = CalculateEntityCount();
            if (entityCount != 1)
                throw new System.InvalidOperationException($"GetSingleton<{typeof(T)}>() requires that exactly one {typeof(T)} exists but there are {entityCount}.");
            #endif

            CompleteDependency();

            T value;
            var iterator = GetArchetypeChunkIterator();
            iterator.MoveNext();
            
            var array = iterator.GetCurrentChunkComponentDataPtr(false, 1);
            UnsafeUtility.CopyPtrToStructure(array, out value);

            return value;
        }

        /// <summary>
        /// Sets the value of a singleton component.
        /// </summary>
        /// <remarks>
        /// For a component to be a singleton, there can be only one instance of that component
        /// in a <see cref="World"/>. The component must be the only component in its archetype
        /// and you cannot use the same type of component as a normal component.
        ///
        /// To create a singleton, create an entity with the singleton component as its only component,
        /// and then use `SetSingleton()` to assign a value.
        ///
        /// For example, if you had a component defined as:
        /// <code>
        /// public struct Singlet: IComponentData{ public int Value; }
        /// </code>
        ///
        /// You could create a singleton as follows:
        ///
        /// <code>
        /// var singletonEntity = entityManager.CreateEntity(typeof(Singlet));
        /// var singletonGroup = entityManager.CreateEntityQuery(typeof(Singlet));
        /// singletonGroup.SetSingleton&lt;Singlet&gt;(new Singlet {Value = 1});
        /// </code>
        ///
        /// You can set and get the singleton value from a EntityQuery or a ComponentSystem.
        /// </remarks>
        /// <param name="value">An instance of type T containing the values to set.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if more than one instance of this component type
        /// exists in the world or the component type appears in more than one archetype.</exception>
        public void SetSingleton<T>(T value)
            where T : struct, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if(GetIndexInEntityQuery(TypeManager.GetTypeIndex<T>()) != 1)
                throw new System.InvalidOperationException($"GetSingleton<{typeof(T)}>() requires that {typeof(T)} is the only component type in its archetype.");

            var entityCount = CalculateEntityCount();
            if (entityCount != 1)
                throw new System.InvalidOperationException($"SetSingleton<{typeof(T)}>() requires that exactly one {typeof(T)} exists but there are {entityCount}.");
#endif

            CompleteDependency();

            var iterator = GetArchetypeChunkIterator();
            iterator.MoveNext();

            var array = iterator.GetCurrentChunkComponentDataPtr(true, 1);
            UnsafeUtility.CopyStructureToPtr(ref value, array);
        }

        internal bool CompareComponents(ComponentType* componentTypes, int count)
        {
            return EntityQueryManager.CompareComponents(componentTypes, count, m_QueryData);
        }

        // @TODO: Define what CompareComponents() does
        /// <summary>
        ///
        /// </summary>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        public bool CompareComponents(ComponentType[] componentTypes)
        {
            fixed (ComponentType* componentTypesPtr = componentTypes)
            {
                return EntityQueryManager.CompareComponents(componentTypesPtr, componentTypes.Length, m_QueryData);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="componentTypes"></param>
        /// <returns></returns>
        public bool CompareComponents(NativeArray<ComponentType> componentTypes)
        {
            return EntityQueryManager.CompareComponents((ComponentType*)componentTypes.GetUnsafeReadOnlyPtr(), componentTypes.Length, m_QueryData);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="queryDesc"></param>
        /// <returns></returns>
        public bool CompareQuery(EntityQueryDesc[] queryDesc)
        {
            return EntityQueryManager.CompareQuery(queryDesc, m_QueryData);
        }

        /// <summary>
        /// Resets this EntityQuery's filter.
        /// </summary>
        /// <remarks>
        /// Removes references to shared component data, if applicable, then resets the filter type to None.
        /// </remarks>
        public void ResetFilter()
        {
            var sharedCount = m_Filter.Shared.Count;
            var sm = ManagedComponentStore;
            for (var i = 0; i < sharedCount; ++i)
                sm.RemoveReference(m_Filter.Shared.SharedComponentIndex[i]);

            m_Filter.Changed.Count = 0;
            m_Filter.Shared.Count = 0;
        }

        /// <summary>
        /// Filters this EntityQuery so that it only selects entities with shared component values
        /// matching the values specified by the `sharedComponent1` parameter.
        /// </summary>
        /// <param name="sharedComponent1">The shared component values on which to filter.</param>
        /// <typeparam name="SharedComponent1">The type of shared component. (The type must also be
        /// one of the types used to create the EntityQuery.</typeparam>
        public void SetSharedComponentFilter<SharedComponent1>(SharedComponent1 sharedComponent1)
            where SharedComponent1 : struct, ISharedComponentData
        {
            ResetFilter();
            AddSharedComponentFilter(sharedComponent1);
        }

        /// <summary>
        /// Filters this EntityQuery based on the values of two separate shared components.
        /// </summary>
        /// <remarks>
        /// The filter only selects entities for which both shared component values
        /// specified by the `sharedComponent1` and `sharedComponent2` parameters match.
        /// </remarks>
        /// <param name="sharedComponent1">Shared component values on which to filter.</param>
        /// <param name="sharedComponent2">Shared component values on which to filter.</param>
        /// <typeparam name="SharedComponent1">The type of shared component. (The type must also be
        /// one of the types used to create the EntityQuery.</typeparam>
        /// <typeparam name="SharedComponent2">The type of shared component. (The type must also be
        /// one of the types used to create the EntityQuery.</typeparam>
        public void SetSharedComponentFilter<SharedComponent1, SharedComponent2>(SharedComponent1 sharedComponent1,
            SharedComponent2 sharedComponent2)
            where SharedComponent1 : struct, ISharedComponentData
            where SharedComponent2 : struct, ISharedComponentData
        {
            ResetFilter();
            AddSharedComponentFilter(sharedComponent1);
            AddSharedComponentFilter(sharedComponent2);
        }

        /// <summary>
        /// Filters out entities in chunks for which the specified component has not changed.
        /// </summary>
        /// <remarks>
        ///     Saves a given ComponentType's index in RequiredComponents in this group's Changed filter.
        /// </remarks>
        /// <param name="componentType">ComponentType to mark as changed on this EntityQuery's filter.</param>
        public void SetChangedVersionFilter(ComponentType componentType)
        {
            ResetFilter();
            AddChangedVersionFilter(componentType);
        }

        internal void SetChangedFilterRequiredVersion(uint requiredVersion)
        {
            m_Filter.RequiredChangeVersion = requiredVersion;
        }

        public void SetChangedVersionFilter(ComponentType[] componentType)
        {
            if (componentType.Length > EntityQueryFilter.ChangedFilter.Capacity)
                throw new ArgumentException(
                    $"EntityQuery.SetFilterChanged accepts a maximum of {EntityQueryFilter.ChangedFilter.Capacity} component array length");
            if (componentType.Length <= 0)
                throw new ArgumentException(
                    $"EntityQuery.SetFilterChanged component array length must be larger than 0");

            ResetFilter();
            for (var i = 0; i != componentType.Length; i++)
                AddChangedVersionFilter(componentType[i]);
        }

        public void AddChangedVersionFilter(ComponentType componentType)
        {
            var newFilterIndex = m_Filter.Changed.Count;
            if (newFilterIndex >= EntityQueryFilter.ChangedFilter.Capacity)
                throw new ArgumentException($"EntityQuery accepts a maximum of {EntityQueryFilter.ChangedFilter.Capacity} changed filters.");

            m_Filter.Changed.Count = newFilterIndex + 1;
            m_Filter.Changed.IndexInEntityQuery[newFilterIndex] = GetIndexInEntityQuery(componentType.TypeIndex);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Filter.AssertValid();
#endif
        }

        public void AddSharedComponentFilter<SharedComponent>(SharedComponent sharedComponent)
            where SharedComponent : struct, ISharedComponentData
        {
            var sm = ManagedComponentStore;

            var newFilterIndex = m_Filter.Shared.Count;
            if (newFilterIndex >= EntityQueryFilter.SharedComponentData.Capacity)
                throw new ArgumentException($"EntityQuery accepts a maximum of {EntityQueryFilter.SharedComponentData.Capacity} shared component filters.");

            m_Filter.Shared.Count = newFilterIndex + 1;
            m_Filter.Shared.IndexInEntityQuery[newFilterIndex] = GetIndexInEntityQuery(TypeManager.GetTypeIndex<SharedComponent>());
            m_Filter.Shared.SharedComponentIndex[newFilterIndex] = sm.InsertSharedComponent(sharedComponent);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Filter.AssertValid();
#endif
        }

        /// <summary>
        ///     Ensures all jobs running on this EntityQuery complete.
        /// </summary>
        public void CompleteDependency()
        {
            m_SafetyManager->CompleteDependenciesNoChecks(m_QueryData->ReaderTypes, m_QueryData->ReaderTypesCount,
                m_QueryData->WriterTypes, m_QueryData->WriterTypesCount);
        }

        /// <summary>
        ///     Combines all dependencies in this EntityQuery into a single JobHandle.
        /// </summary>
        /// <returns>JobHandle that represents the combined dependencies of this EntityQuery</returns>
        public JobHandle GetDependency()
        {
            return m_SafetyManager->GetDependency(m_QueryData->ReaderTypes, m_QueryData->ReaderTypesCount,
                m_QueryData->WriterTypes, m_QueryData->WriterTypesCount);
        }

        /// <summary>
        ///     Adds another job handle to this EntityQuery's dependencies.
        /// </summary>
        public JobHandle AddDependency(JobHandle job)
        {
            return m_SafetyManager->AddDependency(m_QueryData->ReaderTypes, m_QueryData->ReaderTypesCount,
                m_QueryData->WriterTypes, m_QueryData->WriterTypesCount, job);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public int GetCombinedComponentOrderVersion()
        {
            var version = 0;

            for (var i = 0; i < m_QueryData->RequiredComponentsCount; ++i)
                version += m_EntityComponentStore->GetComponentTypeOrderVersion(m_QueryData->RequiredComponents[i].TypeIndex);

            return version;
        }

        internal bool AddReaderWritersToLists(ref UnsafeIntList reading, ref UnsafeIntList writing)
        {
            bool anyAdded = false;
            for (int i = 0; i < m_QueryData->ReaderTypesCount; ++i)
                anyAdded |= CalculateReaderWriterDependency.AddReaderTypeIndex(m_QueryData->ReaderTypes[i], ref reading, ref writing);

            for (int i = 0; i < m_QueryData->WriterTypesCount; ++i)
                anyAdded |=CalculateReaderWriterDependency.AddWriterTypeIndex(m_QueryData->WriterTypes[i], ref reading, ref writing);
            return anyAdded;
        }

        /// <summary>
        /// Syncs the needed types for the filter.
        /// For every type that is change filtered we need to CompleteWriteDependency to avoid race conditions on the
        /// change version of those types
        /// </summary>
        internal void SyncFilterTypes()
        {
            for (int i = 0; i < m_Filter.Changed.Count; ++i)
            {
                var type = m_QueryData->RequiredComponents[m_Filter.Changed.IndexInEntityQuery[i]];
                SafetyManager->CompleteWriteDependency(type.TypeIndex);
            }
        }

        /// <summary>
        /// Syncs the needed types for the filter using the types in UnsafeMatchingArchetypePtrList
        /// This version is used when the EntityQuery is not known
        /// </summary>
        internal static void SyncFilterTypes(ref UnsafeMatchingArchetypePtrList matchingArchetypes, ref EntityQueryFilter filter, ComponentJobSafetyManager* safetyManager)
        {
            if(matchingArchetypes.Length < 1)
                return;

            var match = *matchingArchetypes.Ptr;
            for (int i = 0; i < filter.Changed.Count; ++i)
            {
                var indexInEntityQuery = filter.Changed.IndexInEntityQuery[i];
                var componentIndexInChunk = match->IndexInArchetype[indexInEntityQuery];
                var type = match->Archetype->Types[componentIndexInChunk];
                safetyManager->CompleteWriteDependency(type.TypeIndex);
            }
        }

        /// <summary>
        /// Returns if the entity query has a filter applied to it.
        /// </summary>
        /// <returns>Returns true if the query has a filter, returns false if the query does not have a filter.</returns>
        public bool HasFilter()
        {
            return m_Filter.RequiresMatchesFilter;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("CalculateLength has been renamed to CalculateEntityCount. (RemovedAfter 2019-10-25) (UnityUpgradable) -> CalculateEntityCount()")]
        public int CalculateLength() => throw new NotSupportedException();

       [EditorBrowsable(EditorBrowsableState.Never)]
       [Obsolete("SetFilter has been renamed to SetSharedComponentFilter. (RemovedAfter 2019-11-8) (UnityUpgradable) -> SetSharedComponentFilter(*)")]
       public void SetFilter<SharedComponent1>(SharedComponent1 sharedComponent1) where SharedComponent1 : struct, ISharedComponentData => throw new NotSupportedException();

       [EditorBrowsable(EditorBrowsableState.Never)]
       [Obsolete("SetFilter has been renamed to SetSharedComponentFilter. (RemovedAfter 2019-11-8) (UnityUpgradable) -> SetSharedComponentFilter(*)")]
       public void SetFilter<SharedComponent1, SharedComponent2>(SharedComponent1 sharedComponent1,
           SharedComponent2 sharedComponent2)
           where SharedComponent1 : struct, ISharedComponentData
           where SharedComponent2 : struct, ISharedComponentData
           => throw new NotSupportedException();


       // TODO: remove UNITY_SKIP_UPDATES_WITH_VALIDATION_SUITE block when package test generation is fixed

       #if UNITY_SKIP_UPDATES_WITH_VALIDATION_SUITE

       [EditorBrowsable(EditorBrowsableState.Never)]
       [Obsolete("SetFilterChanged has been renamed to SetChangedVersionFilter. (RemovedAfter 2019-11-8). If you see this in a user project, please remove UNITY_SKIP_UPDATES_WITH_VALIDATION_SUITE from the Unity.Entities assembly definition file.")]
       public void SetFilterChanged(ComponentType componentType) => throw new NotSupportedException();

       [EditorBrowsable(EditorBrowsableState.Never)]
       [Obsolete("SetFilterChanged has been renamed to SetChangedVersionFilter. (RemovedAfter 2019-11-8). If you see this in a user project, please remove UNITY_SKIP_UPDATES_WITH_VALIDATION_SUITE from the Unity.Entities assembly definition file.")]
       public void SetFilterChanged(ComponentType[] componentType) => throw new NotSupportedException();

       #else

       [EditorBrowsable(EditorBrowsableState.Never)]
       [Obsolete("SetFilterChanged has been renamed to SetChangedVersionFilter. (RemovedAfter 2019-11-8) (UnityUpgradable) -> SetChangedVersionFilter()")]
       public void SetFilterChanged(ComponentType componentType) => throw new NotSupportedException();

       [EditorBrowsable(EditorBrowsableState.Never)]
       [Obsolete("SetFilterChanged has been renamed to SetChangedVersionFilter. (RemovedAfter 2019-11-8) (UnityUpgradable) -> SetChangedVersionFilter()")]
       public void SetFilterChanged(ComponentType[] componentType) => throw new NotSupportedException();

       #endif
    }

#if !UNITY_DISABLE_MANAGED_COMPONENTS
    internal static unsafe class EntityQueryManagedComponentExtensions
    {
        /// <summary>
        /// Gets the value of a singleton component.
        /// </summary>
        /// <remarks>A singleton component is a component of which only one instance exists in the world
        /// and which has been set with <see cref="SetSingleton{T}(T)"/>.</remarks>
        /// <typeparam name="T">The component type.</typeparam>
        /// <returns>A copy of the singleton component.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T GetSingleton<T>(this EntityQuery query) where T : class, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (query.GetIndexInEntityQuery(TypeManager.GetTypeIndex<T>()) != 1)
                throw new System.InvalidOperationException($"GetSingleton<{typeof(T)}>() requires that {typeof(T)} is the only component type in its archetype.");

            var entityCount = query.CalculateEntityCount();
            if (entityCount != 1)
                throw new System.InvalidOperationException($"GetSingleton<{typeof(T)}>() requires that exactly one {typeof(T)} exists but there are {entityCount}.");
#endif

            query.CompleteDependency();
            
            var iterator = query.GetArchetypeChunkIterator();
            iterator.MoveNext();

            var archetypeChunk = iterator.CurrentArchetypeChunk;
            var componentType = ComponentType.FromTypeIndex(TypeManager.GetTypeIndex<T>());
            return (T) query.ManagedComponentStore.GetManagedObject(archetypeChunk.m_Chunk, componentType, 0);
        }

        /// <summary>
        /// Sets the value of a singleton component.
        /// </summary>
        /// <remarks>
        /// For a component to be a singleton, there can be only one instance of that component
        /// in a <see cref="World"/>. The component must be the only component in its archetype
        /// and you cannot use the same type of component as a normal component.
        ///
        /// To create a singleton, create an entity with the singleton component as its only component,
        /// and then use `SetSingleton()` to assign a value.
        ///
        /// For example, if you had a component defined as:
        /// <code>
        /// public class Singlet : IComponentData{ public int Value; }
        /// </code>
        ///
        /// You could create a singleton as follows:
        ///
        /// <code>
        /// var singletonEntity = entityManager.CreateEntity(typeof(Singlet));
        /// var singletonGroup = entityManager.CreateEntityQuery(typeof(Singlet));
        /// singletonGroup.SetSingleton&lt;Singlet&gt;(new Singlet {Value = 1});
        /// </code>
        ///
        /// You can set and get the singleton value from a EntityQuery or a ComponentSystem.
        /// </remarks>
        /// <param name="value">An instance of type T containing the values to set.</param>
        /// <typeparam name="T">The component type.</typeparam>
        /// <exception cref="InvalidOperationException">Thrown if more than one instance of this component type
        /// exists in the world or the component type appears in more than one archetype.</exception>
        public static void SetSingleton<T>(this EntityQuery query, T value) where T : class, IComponentData
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (query.GetIndexInEntityQuery(TypeManager.GetTypeIndex<T>()) != 1)
                throw new System.InvalidOperationException($"GetSingleton<{typeof(T)}>() requires that {typeof(T)} is the only component type in its archetype.");

            var entityCount = query.CalculateEntityCount();
            if (entityCount != 1)
                throw new System.InvalidOperationException($"SetSingleton<{typeof(T)}>() requires that exactly one {typeof(T)} exists but there are {entityCount}.");
#endif

            query.CompleteDependency();

            var iterator = query.GetArchetypeChunkIterator();
            iterator.MoveNext();

            var archetypeChunk = iterator.CurrentArchetypeChunk;
            var componentType = ComponentType.FromTypeIndex(TypeManager.GetTypeIndex<T>());
            query.ManagedComponentStore.SetManagedObject(archetypeChunk.m_Chunk, componentType, 0, value);
        }
    }
#endif
}

using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    [Serializable]
    public class DuplicateEntityGuidException : Exception
    {
        public EntityGuid[] DuplicateEntityGuids { get; set; }

        public DuplicateEntityGuidException()
        {
        }

        public DuplicateEntityGuidException(string message)
            : base(message)
        {
        }

        public DuplicateEntityGuidException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    
    [Serializable]
    public struct EntityGuid : IComponentData, IEquatable<EntityGuid>, IComparable<EntityGuid>
    {
        public ulong a;
        public ulong b;

        public static readonly EntityGuid Null = new EntityGuid();

        public static bool operator ==(EntityGuid lhs, EntityGuid rhs)
        {
            return lhs.a == rhs.a && lhs.b == rhs.b;
        }

        public static bool operator !=(EntityGuid lhs, EntityGuid rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityGuid entityGuid ? Equals(entityGuid) : false;
        }

        public bool Equals(EntityGuid other)
        {
            return a == other.a && b == other.b;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (a.GetHashCode() * 397) ^ b.GetHashCode();
            }
        }

        public int CompareTo(EntityGuid other)
        {
            if (a != other.a)
                return a > other.a ? 1 : -1;

            if (b != other.b)
                return b > other.b ? 1 : -1;

            return 0;
        }

        public override string ToString()
        {
            return $"{a:x16}{b:x16}";
        }
    }

    [Flags]
    public enum EntityManagerDifferOptions
    {
        None = 0,

        /// <summary>
        /// If this flag is set; the resulting <see cref="EntityChanges"/> will include the forward change set.
        /// </summary>
        IncludeForwardChangeSet = 1 << 1,

        /// <summary>
        /// If this flag is set; the resulting <see cref="EntityChanges"/> will include the reverse change set.
        ///
        /// This can be applied to the world to reverse the changes (i.e. undo)
        /// </summary>
        IncludeReverseChangeSet = 1 << 2,

        /// <summary>
        /// If this flag is set; the shadow world will be updated with the latest changes.
        /// </summary>
        FastForwardShadowWorld = 1 << 3,

        /// <summary>
        /// If this flag is set; all references to destroyed or missing entities will be set to Entity.Null before computing changes.
        /// </summary>
        /// <remarks>
        /// When applying a change this is needed to patch references to restored entities (they were destroyed but are being brought back by the change set)
        /// </remarks>
        ClearMissingReferences = 1 << 4,

        /// <summary>
        /// If this flag is set; the entire world is checks for duplicate <see cref="EntityGuid"/> components.
        /// </summary>
        /// <remarks>
        /// WARNING The performance of this scales with the number of entities in the world with the <see cref="EntityGuid"/> component.
        /// </remarks>
        ValidateUniqueEntityGuid = 1 << 5,

        /// <summary>
        /// The default set of options used by the <see cref="EntityManagerDiffer"/>
        /// </summary>
        Default = IncludeForwardChangeSet | IncludeReverseChangeSet | FastForwardShadowWorld | ClearMissingReferences |
                  ValidateUniqueEntityGuid
    }

    /// <summary>
    /// The <see cref="EntityManagerDiffer"/> is used to efficiently track changes to a given world over time.
    /// </summary>
    public struct EntityManagerDiffer : IDisposable
    {
        // ReSharper disable once StaticMemberInGenericType
        private static Profiling.ProfilerMarker s_GetChangesMarker = new Profiling.ProfilerMarker("GetChanges");

        private World m_SrcWorld;
        private World m_ShadowWorld;

        private EntityQuery m_SrcWorldEntityQuery;
        private EntityQuery m_ShadowWorldEntityQuery;

        /// <summary>
        /// Blittable type information to be used in bursted jobs.
        /// </summary>
        private TypeInfoStream m_TypeInfoStream;

        /// <summary>
        /// Creates a stateful change tracker over the given world.
        /// </summary>
        /// <param name="srcWorld">The input world to track changes for.</param>
        /// <param name="allocator">Allocator used for the cached state.</param>
        public EntityManagerDiffer(World srcWorld, Allocator allocator)
        {
            m_SrcWorld = srcWorld ?? throw new ArgumentNullException(nameof(srcWorld));
            m_ShadowWorld = new World(srcWorld.Name + " (Shadow)");
            m_SrcWorldEntityQuery = EntityManagerDifferUtility.CreateQuery(srcWorld.EntityManager);
            m_ShadowWorldEntityQuery = EntityManagerDifferUtility.CreateQuery(m_ShadowWorld.EntityManager);
            m_TypeInfoStream = new TypeInfoStream(allocator);
        }

        public void Dispose()
        {
            if (m_ShadowWorld != null && m_ShadowWorld.IsCreated)
                m_ShadowWorld.Dispose();
            m_ShadowWorld = null;
            m_ShadowWorldEntityQuery = null;
    
            if (m_SrcWorld != null && m_SrcWorld.IsCreated)
                m_SrcWorldEntityQuery.Dispose();
            m_SrcWorldEntityQuery = null;
            m_SrcWorld = null;

            m_TypeInfoStream.Dispose();

        }

        /// <summary>
        /// Generates a detailed change set for the world.
        /// All entities to be considered for diffing must have the <see cref="EntityGuid"/> component with a unique value.
        /// The resulting <see cref="EntityChanges"/> must be disposed when no longer needed.
        /// </summary>
        /// <param name="options">A set of options which can be toggled.</param>
        /// <param name="allocator">The allocator to use for the results object.</param>
        /// <returns>A Change set containing the differences between the two worlds.</returns>
        public EntityChanges GetChanges(EntityManagerDifferOptions options, Allocator allocator)
        {
            s_GetChangesMarker.Begin();

            var changes = EntityManagerDifferUtility.GetChanges(
                m_SrcWorld.EntityManager,
                m_SrcWorldEntityQuery,
                m_ShadowWorld.EntityManager,
                m_ShadowWorldEntityQuery,
                m_TypeInfoStream,
                options,
                allocator);

            s_GetChangesMarker.End();

            return changes;
        }

        internal EntityManager ShadowEntityManager
        {
            get { return m_ShadowWorld.EntityManager; }
        }
    }

    /// <summary>
    /// The <see cref="EntityManagerDifferUtility"/> is used to build a set of changes between two worlds.
    /// </summary>
    /// <remarks>
    /// This class can be used to determine both forward and/or reverse changes between the worlds.
    ///
    /// This class relies on the <see cref="EntityGuid"/> to uniquely identify entities, and expects that each entity
    /// will have a unique value for this component. If any duplicate <see cref="EntityGuid"/> values are encountered
    /// a <see cref="DuplicateEntityGuidException"/> will be thrown.
    ///
    /// For tracking changes to a given world over time make sure to use the stateful version <see cref="EntityManagerDiffer"/>
    /// </remarks>
    static class EntityManagerDifferUtility
    {
        /// <summary>
        /// Generates a forward only change set between the given entity managers.
        /// </summary>
        /// <remarks>
        /// No assumptions are made about the relationship between the given worlds. This means a full
        /// deep compare is done to generate the change set.
        /// </remarks>
        /// <param name="srcEntityManager">The src world to compute changes from.</param>
        /// <param name="dstEntityManager">The dst world to compute changes to.</param>
        /// <param name="allocator">Allocator to use for the returned set.</param>
        /// <returns>A set of changes between src and dst worlds.</returns>
        public static EntityChangeSet GetForwardChanges(
            EntityManager srcEntityManager,
            EntityManager dstEntityManager,
            Allocator allocator)
        {
            using (var typeInfoStream = new TypeInfoStream(Allocator.TempJob))
            using (var srcQuery = CreateQuery(srcEntityManager))
            using (var dstQuery = CreateQuery(dstEntityManager))
            {
                var changes = GetChanges(
                    srcEntityManager,
                    srcQuery,
                    dstEntityManager,
                    dstQuery,
                    typeInfoStream,
                    EntityManagerDifferOptions.IncludeForwardChangeSet,
                    allocator);
                
                // @NOTE We are not disposing the changes object itself.
                // In this case since we only allocated the forward and we are returning it to the caller.
                return changes.ForwardChangeSet;
            }
        }

        /// <summary>
        /// Generates a change set between the given entity managers using the given options.
        /// </summary>
        /// <remarks>
        /// No assumptions are made about the relationship between the given worlds. This means a full
        /// deep compare is done to generate the change set.
        /// </remarks>
        /// <param name="srcEntityManager">The src world to compute changes from.</param>
        /// <param name="dstEntityManager">The dst world to compute changes to.</param>
        /// <param name="options">Options to specify to tailor the diffing based on specific requirements. See <see cref="EntityManagerDifferOptions"/> for more information.</param>
        /// <param name="allocator">Allocator to use for the returned set.</param>
        /// <returns>A set of changes between src and dst worlds.</returns>
        public static EntityChangeSet GetChanges(
            EntityManager srcEntityManager,
            EntityManager dstEntityManager,
            EntityManagerDifferOptions options,
            Allocator allocator)
        {
            using (var typeInfoStream = new TypeInfoStream(Allocator.TempJob))
            {
                var changes = GetChanges(
                    srcEntityManager,
                    CreateQuery(srcEntityManager),
                    dstEntityManager,
                    CreateQuery(dstEntityManager),
                    typeInfoStream,
                    options,
                    allocator);

                // @NOTE We are not disposing the changes object itself.
                // In this case since we only allocated the forward and we are returning it to the caller.
                return changes.ForwardChangeSet;
            }
        }

        /// <summary>
        /// Generates a detailed change set between <see cref="srcEntityManager"/> and <see cref="dstEntityManager"/>.
        /// All entities to be considered must have the <see cref="EntityGuid"/> component with a unique value.
        /// The resulting <see cref="EntityChanges"/> must be disposed when no longer needed.
        /// </summary>
        /// <remarks>
        /// For performance the <see cref="dstToSrcSequenceNumbers"/> is used to map chunks between the given worlds. If this is not provided, or is empty
        /// the tracker must evaluate all chunks, entities, and component data.
        ///
        /// When using the <see cref="EntityManagerDifferOptions.FastForwardShadowWorld"/> the destination world must be a direct ancestor to
        /// the source world, and must only be updated using this call or similar methods. There should be no direct changes to destination world.
        /// </remarks>
        internal static unsafe EntityChanges GetChanges(
            EntityManager srcEntityManager,
            EntityQuery srcEntityQuery,
            EntityManager dstEntityManager,
            EntityQuery dstEntityQuery,
            TypeInfoStream typeInfoStream,
            EntityManagerDifferOptions options,
            Allocator allocator)
        {
            var changes = new EntityChanges();

            srcEntityManager.CompleteAllJobs();
            dstEntityManager.CompleteAllJobs();
            
            if (options != EntityManagerDifferOptions.None)
            {
                var includeForwardChangeSet = (options & EntityManagerDifferOptions.IncludeForwardChangeSet) ==
                                              EntityManagerDifferOptions.IncludeForwardChangeSet;
                var includeReverseChangeSet = (options & EntityManagerDifferOptions.IncludeReverseChangeSet) ==
                                              EntityManagerDifferOptions.IncludeReverseChangeSet;
                var fastForwardShadowWorld = (options & EntityManagerDifferOptions.FastForwardShadowWorld) ==
                                             EntityManagerDifferOptions.FastForwardShadowWorld;
                var clearMissingReferences = (options & EntityManagerDifferOptions.ClearMissingReferences) ==
                                             EntityManagerDifferOptions.ClearMissingReferences;
                var validateUniqueEntityGuids = (options & EntityManagerDifferOptions.ValidateUniqueEntityGuid) ==
                                                EntityManagerDifferOptions.ValidateUniqueEntityGuid;

                // Query chunks that should be considered for change tracking.
                using (var srcChunks = srcEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
                using (var dstChunks = dstEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
                {
                    if (clearMissingReferences)
                    {
                        ArchetypeChunkChangeUtility.ClearMissingReferences(srcChunks,
                            srcEntityManager.EntityComponentStore, srcEntityManager.GlobalSystemVersion,
                            typeInfoStream);
                    }

                    // Compare the chunks and get a set of all changed chunks.
                    // @NOTE A modified chunk will appear as destroyed and then created.
                    using (var archetypeChunkChanges = ArchetypeChunkChangeUtility.GetArchetypeChunkChanges(srcChunks, dstChunks, Allocator.TempJob))
                    {
                        // If we have no chunk-level changes then there is no work to be done.
                        if (archetypeChunkChanges.HasChanges)
                        {
                            if (includeForwardChangeSet || includeReverseChangeSet)
                            {
                                BuildComponentDataToEntityLookupTask<EntityGuid> buildSrcEntityGuidToEntityLookupTask =
                                    default;

                                try
                                {
                                    using (var buildSrcEntitiesTask =
                                        new BuildEntityInChunkWithComponentTask<EntityGuid>(
                                            archetypeChunkChanges.CreatedSrcChunks, Allocator.TempJob))
                                    using (var buildDstEntitiesTask =
                                        new BuildEntityInChunkWithComponentTask<EntityGuid>(
                                            archetypeChunkChanges.DestroyedDstChunks, Allocator.TempJob))
                                    {
                                        var handle = JobHandle.CombineDependencies(buildSrcEntitiesTask.Schedule(),
                                            buildDstEntitiesTask.Schedule());

                                        if (validateUniqueEntityGuids)
                                        {
                                            // Validation is expensive since we must run over the entire world. This is opt in.
                                            buildSrcEntityGuidToEntityLookupTask =
                                                new BuildComponentDataToEntityLookupTask<EntityGuid>(
                                                    srcEntityQuery.CalculateEntityCount(), Allocator.TempJob);
                                            handle = JobHandle.CombineDependencies(handle,
                                                buildSrcEntityGuidToEntityLookupTask.Schedule(srcChunks));
                                        }

                                        handle.Complete();

                                        if (validateUniqueEntityGuids &&
                                            TryGetDuplicateComponents(
                                                buildSrcEntityGuidToEntityLookupTask.GetComponentDataToEntityMap(),
                                                out var duplicates))
                                        {
                                            throw new DuplicateEntityGuidException(message:
                                                $"Found {duplicates.Length} {nameof(EntityGuid)} components that are shared by more than one Entity, " +
                                                $"see the {nameof(DuplicateEntityGuidException.DuplicateEntityGuids)} property for more information.")
                                            {
                                                DuplicateEntityGuids = duplicates
                                            };
                                        }

                                        var srcState = new WorldState(
                                            srcEntityManager,
                                            buildSrcEntitiesTask.GetEntities()
                                        );

                                        var dstState = new WorldState(
                                            dstEntityManager,
                                            buildDstEntitiesTask.GetEntities()
                                        );

                                        BuildChangeSetTask buildForwardChangeSetTask = default;
                                        BuildChangeSetTask buildReverseChangeSetTask = default;

                                        try
                                        {
                                            JobHandle buildForwardChangeSetJob = default;
                                            JobHandle buildReverseChangeSetJob = default;

                                            if (includeForwardChangeSet)
                                            {
                                                buildForwardChangeSetTask = new BuildChangeSetTask(dstState, srcState,
                                                    typeInfoStream, Allocator.TempJob);
                                                buildForwardChangeSetJob = buildForwardChangeSetTask.Schedule();
                                            }

                                            if (includeReverseChangeSet)
                                            {
                                                buildReverseChangeSetTask = new BuildChangeSetTask(srcState, dstState,
                                                    typeInfoStream, Allocator.TempJob);
                                                buildReverseChangeSetJob = buildReverseChangeSetTask.Schedule();
                                            }

                                            JobHandle.CombineDependencies(buildForwardChangeSetJob,
                                                buildReverseChangeSetJob).Complete();

                                            changes = new EntityChanges(
                                                includeForwardChangeSet
                                                    ? buildForwardChangeSetTask.GetChangeSet(allocator)
                                                    : default,
                                                includeReverseChangeSet
                                                    ? buildReverseChangeSetTask.GetChangeSet(allocator)
                                                    : default
                                            );
                                        }
                                        finally
                                        {
                                            if (buildForwardChangeSetTask.IsCreated)
                                            {
                                                buildForwardChangeSetTask.Dispose();
                                            }

                                            if (buildReverseChangeSetTask.IsCreated)
                                            {
                                                buildReverseChangeSetTask.Dispose();
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    if (buildSrcEntityGuidToEntityLookupTask.IsCreated)
                                    {
                                        buildSrcEntityGuidToEntityLookupTask.Dispose();
                                    }
                                }
                            }

                            if (fastForwardShadowWorld)
                            {
                                CopyAndReplaceChunks(srcEntityManager, dstEntityManager, dstEntityQuery, archetypeChunkChanges);
                            }
                        }
                    }
                }
            }
            
            return changes;
        }

        internal static unsafe void CopyAndReplaceChunks(EntityManager srcEntityManager, EntityManager dstEntityManager, EntityQuery dstEntityQuery, ArchetypeChunkChanges archetypeChunkChanges)
        {
            var archetypeChanges = dstEntityManager.EntityComponentStore->BeginArchetypeChangeTracking();

            var destroyedChunks = archetypeChunkChanges.DestroyedDstChunks.Chunks;

            for (var i = 0; i < destroyedChunks.Length; i++)
            {
                var dstChunk = destroyedChunks[i].m_Chunk;
                //@TODO: Review usage of this function. It seems dodgy. Some test coverage around this
                dstEntityManager.DestroyChunkForDiffing(dstChunk);
            }

            var createdChunks = archetypeChunkChanges.CreatedSrcChunks.Chunks;

            var dstClonedChunks = new NativeArray<ArchetypeChunk>(createdChunks.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < createdChunks.Length; i++)
            {
                var srcChunk = createdChunks[i].m_Chunk;

                // Do a full clone of this chunk
                var dstChunk = CloneChunkWithoutAllocatingEntities(
                    dstEntityManager,
                    srcChunk,
                    srcEntityManager.ManagedComponentStore);

                dstClonedChunks[i] = new ArchetypeChunk {m_Chunk = dstChunk};
            }

            // Ensure capacity in the dst world before we start linking entities.
            dstEntityManager.EntityComponentStore->ReallocCapacity(srcEntityManager.EntityCapacity);
            dstEntityManager.EntityComponentStore->CopyNextFreeEntityIndex(srcEntityManager.EntityComponentStore);

            new PatchClonedChunks
            {
                SrcChunks = createdChunks,
                DstChunks = dstClonedChunks,
                DstEntityComponentStore = dstEntityManager.EntityComponentStore
            }.Schedule(createdChunks.Length, 64).Complete();

            
            dstClonedChunks.Dispose();

            var changedArchetypes = dstEntityManager.EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
            dstEntityManager.EntityQueryManager.AddAdditionalArchetypes(changedArchetypes);

            //@TODO-opt: use a query that searches for all chunks that have chunk components on it
            //@TODO-opt: Move this into a job
            // Any chunk might have been recreated, so the ChunkHeader might be invalid 
            using (var allDstChunks = dstEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
            {
                foreach (var chunk in allDstChunks)
                {
                    var metaEntity = chunk.m_Chunk->metaChunkEntity;
                    if (metaEntity != Entity.Null)
                    {
                        if (dstEntityManager.Exists(metaEntity))
                            dstEntityManager.SetComponentData(metaEntity, new ChunkHeader {ArchetypeChunk = chunk});
                    }
                }
            }
            
            srcEntityManager.EntityComponentStore->IncrementGlobalSystemVersion();
            dstEntityManager.EntityComponentStore->IncrementGlobalSystemVersion();
        }

        [BurstCompile]
        private unsafe struct CountDuplicates<TComponent> : IJob
            where TComponent : struct, IEquatable<TComponent>
        {
            [NativeDisableUnsafePtrRestriction] public int* DuplicateCount;
            [ReadOnly] public NativeArray<TComponent> Components;
            [ReadOnly] public NativeMultiHashMap<TComponent, Entity> ComponentToEntity;

            public void Execute()
            {
                var count = 0;
                for (var i = 0; i < Components.Length; i++)
                {
                    ComponentToEntity.TryGetFirstValue(Components[i], out _, out var iterator);

                    if (ComponentToEntity.TryGetNextValue(out _, ref iterator))
                    {
                        count++;
                    }
                }

                *DuplicateCount = count;
            }
        }

        private static unsafe bool TryGetDuplicateComponents<TComponent>(
            NativeMultiHashMap<TComponent, Entity> componentDataToEntityLookup, out TComponent[] duplicates)
            where TComponent : struct, IEquatable<TComponent>
        {
            var duplicateCount = 0;

            using (var keys = componentDataToEntityLookup.GetKeyArray(Allocator.TempJob))
            {
                new CountDuplicates<TComponent>
                {
                    DuplicateCount = &duplicateCount,
                    Components = keys,
                    ComponentToEntity = componentDataToEntityLookup
                }.Run();

                if (duplicateCount > 0)
                {
                    duplicates = new TComponent[duplicateCount];

                    var duplicateIndex = 0;
                    for (var i = 0; i < keys.Length; i++)
                    {
                        componentDataToEntityLookup.TryGetFirstValue(keys[i], out _, out var iterator);

                        if (componentDataToEntityLookup.TryGetNextValue(out _, ref iterator))
                        {
                            duplicates[duplicateIndex++] = keys[i];
                        }
                    }

                    return true;
                }
            }

            duplicates = null;
            return false;
        }

        [BurstCompile]
        private unsafe struct PatchClonedChunks : IJobParallelFor
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> SrcChunks;
            [ReadOnly] public NativeArray<ArchetypeChunk> DstChunks;

            [NativeDisableUnsafePtrRestriction] public EntityComponentStore* DstEntityComponentStore;

            public void Execute(int index)
            {
                var srcChunk = SrcChunks[index].m_Chunk;
                var dstChunk = DstChunks[index].m_Chunk;

                var archetype = srcChunk->Archetype;
                var typeCount = archetype->TypesCount;

                for (var typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    //@TODO: Overridable dst system version here?
                    dstChunk->SetChangeVersion(typeIndex, srcChunk->GetChangeVersion(typeIndex));
                }

                DstEntityComponentStore->AddExistingEntitiesInChunk(dstChunk);
            }
        }

        /// <remarks>
        /// This is essentially a copy paste of <see cref="EntityManagerMoveEntitiesUtility.CloneChunkForDiffing"/> except skipping the `AllocateEntities` step.
        /// </remarks>
        private static unsafe Chunk* CloneChunkWithoutAllocatingEntities(EntityManager dstEntityManager, Chunk* srcChunk, ManagedComponentStore srcManagedComponentStore)
        {
            var dstEntityComponentStore = dstEntityManager.EntityComponentStore;
            var dstManagedComponentStore = dstEntityManager.ManagedComponentStore;

            // Copy shared component data
            var dstSharedIndices = stackalloc int[srcChunk->Archetype->NumSharedComponents];
            srcChunk->SharedComponentValues.CopyTo(dstSharedIndices, 0, srcChunk->Archetype->NumSharedComponents);
            dstManagedComponentStore.CopySharedComponents(srcManagedComponentStore, dstSharedIndices, srcChunk->Archetype->NumSharedComponents);

            //@TODO: Why don't we memcpy the whole chunk. So we include all extra fields???
            
            // Allocate a new chunk
            var srcArchetype = srcChunk->Archetype;
            var dstArchetype = dstEntityComponentStore->GetOrCreateArchetype(srcArchetype->Types, srcArchetype->TypesCount);

            var dstChunk = dstEntityComponentStore->GetCleanChunkNoMetaChunk(dstArchetype, dstSharedIndices);
            dstManagedComponentStore.Playback(ref dstEntityComponentStore->ManagedChangesTracker);

            dstChunk->metaChunkEntity = srcChunk->metaChunkEntity;
            
            // Release any references obtained by GetCleanChunk & CopySharedComponents
            for (var i = 0; i < srcChunk->Archetype->NumSharedComponents; i++)
                dstManagedComponentStore.RemoveReference(dstSharedIndices[i]);

            dstEntityComponentStore->SetChunkCountKeepMetaChunk(dstChunk, srcChunk->Count);
            dstManagedComponentStore.Playback(ref dstEntityComponentStore->ManagedChangesTracker);

            dstChunk->Archetype->EntityCount += srcChunk->Count;

            var copySize = Chunk.GetChunkBufferSize();
            UnsafeUtility.MemCpy((byte*) dstChunk + Chunk.kBufferOffset, (byte*) srcChunk + Chunk.kBufferOffset, copySize);

            //@TODO: Class components should be duplicated instead of copied by ref?
            if (dstChunk->ManagedArrayIndex != -1)
                ManagedComponentStore.CopyManagedObjects(srcManagedComponentStore, srcChunk->Archetype, srcChunk->ManagedArrayIndex, srcChunk->Capacity, 0, dstManagedComponentStore, dstChunk->Archetype, dstChunk->ManagedArrayIndex, dstChunk->Capacity, 0, srcChunk->Count);
            
            BufferHeader.PatchAfterCloningChunk(dstChunk);
            dstChunk->SequenceNumber = srcChunk->SequenceNumber;

            return dstChunk;
        }

        /// <summary>
        /// Returns a query on the world for all entities with the <see cref="EntityGuid"/>
        /// </summary>
        internal static EntityQuery CreateQuery(EntityManager entityManager)
        {
            return entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(EntityGuid)
                },
                Options = EntityQueryOptions.IncludeDisabled | EntityQueryOptions.IncludePrefab
            });
        }
    }
}
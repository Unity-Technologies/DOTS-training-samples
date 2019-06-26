using System;
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
        Default = IncludeForwardChangeSet | IncludeReverseChangeSet | FastForwardShadowWorld | ClearMissingReferences | ValidateUniqueEntityGuid
    }
    
    /// <summary>
    /// The <see cref="EntityManagerDiffer"/> is used to efficiently track changes to a given world over time.
    /// </summary>
    public struct EntityManagerDiffer : IDisposable
    {
        // ReSharper disable once StaticMemberInGenericType
        private static Profiling.ProfilerMarker s_GetChangesMarker = new Profiling.ProfilerMarker("GetChanges");
        
        private readonly World m_SrcWorld;
        private readonly World m_DstWorld;

        private readonly EntityQuery m_SrcWorldEntityQuery;
        private readonly EntityQuery m_DstWorldEntityQuery;
        
        /// <summary>
        /// Mapping of chunk sequence numbers from the <see cref="m_SrcWorld"/> to the <see cref="m_DstWorld"/>
        /// </summary>
        private NativeHashMap<ulong, ulong> m_DstChunkToSrcChunkSequenceNumbers;

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
            m_DstWorld = new World(srcWorld.Name + " (Shadow)");
            m_SrcWorldEntityQuery = EntityManagerDifferUtility.CreateQuery(srcWorld.EntityManager);
            m_DstWorldEntityQuery = EntityManagerDifferUtility.CreateQuery(m_DstWorld.EntityManager);
            m_DstChunkToSrcChunkSequenceNumbers = new NativeHashMap<ulong, ulong>(64, allocator);
            m_TypeInfoStream = new TypeInfoStream(allocator);
        }
		
        public void Dispose()
        {
            m_DstChunkToSrcChunkSequenceNumbers.Dispose();
            m_DstWorldEntityQuery.Dispose();
            m_SrcWorldEntityQuery.Dispose();
            m_DstWorld.Dispose();
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
                m_DstWorld.EntityManager,
                m_DstWorldEntityQuery,
                m_DstChunkToSrcChunkSequenceNumbers,
                m_TypeInfoStream,
                options,
                allocator);
            
            s_GetChangesMarker.End();

            unsafe
            {
                m_SrcWorld.EntityManager.EntityComponentStore->IncrementGlobalSystemVersion();
            }

            return changes;
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
    public static class EntityManagerDifferUtility
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
            using (var dstToSrcSequenceNumbers = new NativeHashMap<ulong, ulong>(1, Allocator.TempJob))
            using (var typeInfoStream = new TypeInfoStream(Allocator.TempJob))
            {
                var changes = GetChanges(
                    srcEntityManager,
                    CreateQuery(srcEntityManager),
                    dstEntityManager,
                    CreateQuery(dstEntityManager),
                    dstToSrcSequenceNumbers,
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
            using (var dstToSrcSequenceNumbers = new NativeHashMap<ulong, ulong>(1, Allocator.TempJob))
            using (var typeInfoStream = new TypeInfoStream(Allocator.TempJob))
            {
                var changes = GetChanges(
                    srcEntityManager,
                    CreateQuery(srcEntityManager),
                    dstEntityManager,
                    CreateQuery(dstEntityManager),
                    dstToSrcSequenceNumbers,
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
            NativeHashMap<ulong, ulong> dstToSrcSequenceNumbers,
            TypeInfoStream typeInfoStream,
            EntityManagerDifferOptions options,
            Allocator allocator)
        {
            var changes = new EntityChanges();

            if (options != EntityManagerDifferOptions.None)
            {
                var includeForwardChangeSet = (options & EntityManagerDifferOptions.IncludeForwardChangeSet) == EntityManagerDifferOptions.IncludeForwardChangeSet;
                var includeReverseChangeSet = (options & EntityManagerDifferOptions.IncludeReverseChangeSet) == EntityManagerDifferOptions.IncludeReverseChangeSet;
                var fastForwardShadowWorld = (options & EntityManagerDifferOptions.FastForwardShadowWorld) == EntityManagerDifferOptions.FastForwardShadowWorld;
                var clearMissingReferences = (options & EntityManagerDifferOptions.ClearMissingReferences) == EntityManagerDifferOptions.ClearMissingReferences;
                var validateUniqueEntityGuids = (options & EntityManagerDifferOptions.ValidateUniqueEntityGuid) == EntityManagerDifferOptions.ValidateUniqueEntityGuid;

                // Query chunks that should be considered for change tracking.
                using (var srcChunks = srcEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
                using (var dstChunks = dstEntityQuery.CreateArchetypeChunkArray(Allocator.TempJob))
                {
                    if (clearMissingReferences)
                    {
                        ArchetypeChunkChangeUtility.ClearMissingReferences(srcChunks, srcEntityManager.EntityComponentStore, srcEntityManager.GlobalSystemVersion, typeInfoStream);
                    }

                    // Compare the chunks and get a set of all changed chunks.
                    // @NOTE A modified chunk will appear as destroyed and then created.
                    using (var archetypeChunkChanges = ArchetypeChunkChangeUtility.GetArchetypeChunkChanges(srcChunks, dstChunks, dstToSrcSequenceNumbers, Allocator.TempJob))
                    {
                        // If we have no chunk-level changes then there is no work to be done.
                        if (archetypeChunkChanges.HasChanges)
                        {
                            if (includeForwardChangeSet || includeReverseChangeSet)
                            {
                                BuildComponentDataToEntityLookupTask<EntityGuid> buildSrcEntityGuidToEntityLookupTask = default;

                                try
                                {
                                    using (var buildSrcEntitiesTask = new BuildEntityInChunkWithComponentTask<EntityGuid>(archetypeChunkChanges.CreatedSrcChunks, Allocator.TempJob))
                                    using (var buildDstEntitiesTask = new BuildEntityInChunkWithComponentTask<EntityGuid>(archetypeChunkChanges.DestroyedDstChunks, Allocator.TempJob))
                                    {
                                        var handle = JobHandle.CombineDependencies(buildSrcEntitiesTask.Schedule(),
                                                                                   buildDstEntitiesTask.Schedule());

                                        if (validateUniqueEntityGuids)
                                        {
                                            // Validation is expensive since we must run over the entire world. This is opt in.
                                            buildSrcEntityGuidToEntityLookupTask = new BuildComponentDataToEntityLookupTask<EntityGuid>(srcEntityQuery.CalculateLength(), Allocator.TempJob);
                                            handle = JobHandle.CombineDependencies(handle, buildSrcEntityGuidToEntityLookupTask.Schedule(srcChunks));
                                        }

                                        handle.Complete();

                                        if (validateUniqueEntityGuids && TryGetDuplicateComponents(buildSrcEntityGuidToEntityLookupTask.GetComponentDataToEntityMap(), out var duplicates))
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
                                                buildForwardChangeSetTask = new BuildChangeSetTask(dstState, srcState, typeInfoStream, Allocator.TempJob);
                                                buildForwardChangeSetJob = buildForwardChangeSetTask.Schedule();
                                            }

                                            if (includeReverseChangeSet)
                                            {
                                                buildReverseChangeSetTask = new BuildChangeSetTask(srcState, dstState, typeInfoStream, Allocator.TempJob);
                                                buildReverseChangeSetJob = buildReverseChangeSetTask.Schedule();
                                            }

                                            JobHandle.CombineDependencies(buildForwardChangeSetJob, buildReverseChangeSetJob).Complete();

                                            changes = new EntityChanges(
                                                includeForwardChangeSet ? buildForwardChangeSetTask.GetChangeSet(allocator) : default,
                                                includeReverseChangeSet ? buildReverseChangeSetTask.GetChangeSet(allocator) : default
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
                                var archetypeChanges = dstEntityManager.EntityComponentStore->BeginArchetypeChangeTracking();

                                var destroyedChunks = archetypeChunkChanges.DestroyedDstChunks.Chunks;

                                for (var i = 0; i < destroyedChunks.Length; i++)
                                {
                                    var dstChunk = destroyedChunks[i].m_Chunk;

                                    dstToSrcSequenceNumbers.Remove(dstChunk->SequenceNumber);

                                    EntityManagerMoveEntitiesUtility.DestroyChunkForDiffing(dstChunk,
                                                                                            dstEntityManager.EntityComponentStore,
                                                                                            dstEntityManager.ManagedComponentStore);
                                }

                                var createdChunks = archetypeChunkChanges.CreatedSrcChunks.Chunks;

                                var dstClonedChunks = new NativeArray<ArchetypeChunk>(createdChunks.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
                                {
                                    for (var i = 0; i < createdChunks.Length; i++)
                                    {
                                        var srcChunk = createdChunks[i].m_Chunk;

                                        // Do a full clone of this chunk
                                        var dstChunk = CloneChunkWithoutAllocatingEntities(
                                            dstEntityManager,
                                            srcChunk,
                                            srcEntityManager.ManagedComponentStore);

                                        dstClonedChunks[i] = new ArchetypeChunk {m_Chunk = dstChunk};
                                        dstToSrcSequenceNumbers.TryAdd(dstChunk->SequenceNumber, srcChunk->SequenceNumber);
                                    }

                                    // Ensure capacity since we can not resize a concurrent hash map in a parallel job.
                                    dstToSrcSequenceNumbers.Capacity = Math.Max(
                                        dstToSrcSequenceNumbers.Capacity,
                                        dstToSrcSequenceNumbers.Length + createdChunks.Length);

                                    // Ensure capacity in the dst world before we start linking entities.
                                    dstEntityManager.EntityComponentStore->ReallocCapacity(srcEntityManager.EntityCapacity);

                                    new PatchClonedChunks
                                    {
                                        SrcChunks = createdChunks,
                                        DstChunks = dstClonedChunks,
                                        DstChunkToSrcChunkSequenceHashMap = dstToSrcSequenceNumbers.ToConcurrent(),
                                        DstEntityComponentStore = dstEntityManager.EntityComponentStore
                                    }.Schedule(createdChunks.Length, 64).Complete();
                                }
                                dstClonedChunks.Dispose();

                                var changedArchetypes = dstEntityManager.EntityComponentStore->EndArchetypeChangeTracking(archetypeChanges);
                                dstEntityManager.EntityGroupManager.AddAdditionalArchetypes(changedArchetypes);
                            }
                        }
                    }
                }
            }

            return changes;
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

        private static unsafe bool TryGetDuplicateComponents<TComponent>(NativeMultiHashMap<TComponent, Entity> componentDataToEntityLookup, out TComponent[] duplicates) 
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

            public NativeHashMap<ulong, ulong>.Concurrent DstChunkToSrcChunkSequenceHashMap;

            public void Execute(int index)
            {
                var srcChunk = SrcChunks[index].m_Chunk;
                var dstChunk = DstChunks[index].m_Chunk;

                var archetype = srcChunk->Archetype;
                var typeCount = archetype->TypesCount;

                for (var typeIndex = 0; typeIndex < typeCount; typeIndex++)
                {
                    dstChunk->SetChangeVersion(typeIndex, srcChunk->GetChangeVersion(typeIndex));
                }

                DstEntityComponentStore->AddExistingEntitiesInChunk(dstChunk);
                DstChunkToSrcChunkSequenceHashMap.TryAdd(dstChunk->SequenceNumber, srcChunk->SequenceNumber);
            }
        }

        /// <remarks>
        /// This is essentially a copy paste of <see cref="EntityManagerMoveEntitiesUtility.CloneChunkForDiffing"/> except skipping the `AllocateEntities` step.
        /// </remarks>
        private static unsafe Chunk* CloneChunkWithoutAllocatingEntities(EntityManager dstEntityManager, Chunk* srcChunk, ManagedComponentStore srcSharedComponentManager)
        {
            var dstEntityComponentStore = dstEntityManager.EntityComponentStore;
            var dstManagedComponentStore = dstEntityManager.ManagedComponentStore;

            // Copy shared component data
            var dstSharedIndices = stackalloc int[srcChunk->Archetype->NumSharedComponents];
            srcChunk->SharedComponentValues.CopyTo(dstSharedIndices, 0, srcChunk->Archetype->NumSharedComponents);
            dstManagedComponentStore.CopySharedComponents(srcSharedComponentManager, dstSharedIndices, srcChunk->Archetype->NumSharedComponents);

            // Allocate a new chunk
            var srcArchetype = srcChunk->Archetype;
            var dstArchetype = EntityManagerCreateArchetypeUtility.GetOrCreateArchetype(srcArchetype->Types,
                                                                                        srcArchetype->TypesCount, dstEntityComponentStore);

            var dstChunk = EntityManagerCreateDestroyEntitiesUtility.GetCleanChunk(
                dstArchetype,
                dstSharedIndices,
                dstEntityComponentStore,
                dstManagedComponentStore);

            // Release any references obtained by GetCleanChunk & CopySharedComponents
            for (var i = 0; i < srcChunk->Archetype->NumSharedComponents; i++)
            {
                dstManagedComponentStore.RemoveReference(dstSharedIndices[i]);
            }

            EntityManagerCreateDestroyEntitiesUtility.SetChunkCount(
                dstChunk,
                srcChunk->Count,
                dstEntityComponentStore,
                dstManagedComponentStore);

            dstChunk->Archetype->EntityCount += srcChunk->Count;

            var copySize = Chunk.GetChunkBufferSize();
            UnsafeUtility.MemCpy((byte*) dstChunk + Chunk.kBufferOffset, (byte*) srcChunk + Chunk.kBufferOffset, copySize);

            BufferHeader.PatchAfterCloningChunk(dstChunk);

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
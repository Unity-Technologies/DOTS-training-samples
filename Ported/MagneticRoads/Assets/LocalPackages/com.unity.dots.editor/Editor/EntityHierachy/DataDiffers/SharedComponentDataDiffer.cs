using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;

namespace Unity.Entities.Editor
{
    class SharedComponentDataDiffer : IDisposable
    {
        static readonly ProfilerMarker k_GatherComponentChanges = new ProfilerMarker($"{nameof(SharedComponentDataDiffer)}.{nameof(GatherComponentChanges)}");
        static readonly ProfilerMarker k_GatherComponentChangesJobWorkScheduling = new ProfilerMarker($"{nameof(SharedComponentDataDiffer)}.{nameof(GatherComponentChanges)} job based diff - scheduling");
        static readonly ProfilerMarker k_GatherComponentChangesJobWorkCompleting = new ProfilerMarker($"{nameof(SharedComponentDataDiffer)}.{nameof(GatherComponentChanges)} job based diff - completing");
        static readonly ProfilerMarker k_GatherComponentChangesResultProcessing = new ProfilerMarker($"{nameof(SharedComponentDataDiffer)}.{nameof(GatherComponentChanges)} result processing");
        static readonly ProfilerMarker k_GatherComponentChangesBufferAlloc = new ProfilerMarker($"{nameof(SharedComponentDataDiffer)}.{nameof(GatherComponentChanges)} buffer alloc");
        static readonly ProfilerMarker k_GatherComponentChangesResultBufferAlloc = new ProfilerMarker($"{nameof(SharedComponentDataDiffer)}.{nameof(GatherComponentChanges)} result buffers alloc");
        static readonly ProfilerMarker k_GatherComponentChangesResultBufferDispose = new ProfilerMarker($"{nameof(SharedComponentDataDiffer)}.{nameof(GatherComponentChanges)} result buffers dispose");

        readonly int m_TypeIndex;
        readonly object m_DefaultComponentDataValue;
        readonly List<object> m_ManagedComponentStoreStateCopy = new List<object>();

        NativeHashMap<ulong, int> m_ManagedComponentIndexInCopyByChunk;
        NativeHashMap<ulong, ShadowChunk> m_ShadowChunks;
        NativeList<ShadowChunk> m_AllocatedShadowChunksForTheFrame;
        NativeList<ChangesCollector> m_GatheredChanges;
        NativeList<ulong> m_RemovedShadowChunks;

        unsafe struct ShadowChunk
        {
            public int EntityCount;
            public uint Version;
            public Entity* EntityDataBuffer;
        }

        public SharedComponentDataDiffer(ComponentType componentType)
        {
            if (!CanWatch(componentType))
                throw new ArgumentException($"{nameof(SharedComponentDataDiffer)} only supports {nameof(ISharedComponentData)} components.", nameof(componentType));

            WatchedComponentType = componentType;
            m_TypeIndex = componentType.TypeIndex;
            m_ManagedComponentIndexInCopyByChunk = new NativeHashMap<ulong, int>(100, Allocator.Persistent);
            m_ShadowChunks = new NativeHashMap<ulong, ShadowChunk>(100, Allocator.Persistent);
            m_DefaultComponentDataValue = Activator.CreateInstance(componentType.GetManagedType());

            m_AllocatedShadowChunksForTheFrame = new NativeList<ShadowChunk>(16, Allocator.Persistent);
            m_GatheredChanges = new NativeList<ChangesCollector>(16, Allocator.Persistent);
            m_RemovedShadowChunks = new NativeList<ulong>(Allocator.Persistent);
        }

        public ComponentType WatchedComponentType { get; }

        public static bool CanWatch(ComponentType componentType)
            => TypeManager.GetTypeInfo(componentType.TypeIndex).Category == TypeManager.TypeCategory.ISharedComponentData;

        public unsafe void Dispose()
        {
            using (var array = m_ShadowChunks.GetValueArray(Allocator.Temp))
            {
                for (var i = 0; i < array.Length; i++)
                {
                    UnsafeUtility.Free(array[i].EntityDataBuffer, Allocator.Persistent);
                }
            }

            m_ManagedComponentIndexInCopyByChunk.Dispose();
            m_ShadowChunks.Dispose();
            m_AllocatedShadowChunksForTheFrame.Dispose();
            m_GatheredChanges.Dispose();
            m_RemovedShadowChunks.Dispose();
        }

        public unsafe ComponentChanges GatherComponentChanges(EntityManager entityManager, EntityQuery query, Allocator allocator)
        {
            using (k_GatherComponentChanges.Auto())
            {
                var chunks = query.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out var chunksJobHandle);
                k_GatherComponentChangesBufferAlloc.Begin();
                m_AllocatedShadowChunksForTheFrame.Clear();
                m_AllocatedShadowChunksForTheFrame.ResizeUninitialized(chunks.Length);
                m_GatheredChanges.Clear();
                m_GatheredChanges.Resize(chunks.Length, NativeArrayOptions.ClearMemory);
                m_RemovedShadowChunks.Clear();
                k_GatherComponentChangesBufferAlloc.End();
                k_GatherComponentChangesResultBufferAlloc.Begin();
                var sharedComponentDataBuffer = new NativeList<GCHandle>(allocator);
                var addedEntities = new NativeList<Entity>(allocator);
                var addedEntitiesMapping = new NativeList<int>(allocator);
                var removedEntities = new NativeList<Entity>(allocator);
                var removedEntitiesMapping = new NativeList<int>(allocator);

                var indexOfFirstAdded = 0;
                var indicesInManagedComponentStore = new NativeList<int>(Allocator.TempJob);
                k_GatherComponentChangesResultBufferAlloc.End();

                k_GatherComponentChangesJobWorkScheduling.Begin();
                var changesJobHandle = new GatherChangesJob
                {
                    TypeIndex = m_TypeIndex,
                    Chunks = chunks,
                    ShadowChunksBySequenceNumber = m_ShadowChunks,
                    GatheredChanges = (ChangesCollector*)m_GatheredChanges.GetUnsafeList()->Ptr
                }.Schedule(chunks.Length, 1, chunksJobHandle);

                var allocateNewShadowChunksJobHandle = new AllocateNewShadowChunksJob
                {
                    TypeIndex = m_TypeIndex,
                    Chunks = chunks,
                    ShadowChunksBySequenceNumber = m_ShadowChunks,
                    AllocatedShadowChunks = (ShadowChunk*)m_AllocatedShadowChunksForTheFrame.GetUnsafeList()->Ptr
                }.Schedule(chunks.Length, 1, chunksJobHandle);

                var copyJobHandle = new CopyStateToShadowChunksJob
                {
                    TypeIndex = m_TypeIndex,
                    Chunks = chunks,
                    ShadowChunksBySequenceNumber = m_ShadowChunks,
                    AllocatedShadowChunks = (ShadowChunk*)m_AllocatedShadowChunksForTheFrame.GetUnsafeList()->Ptr,
                    RemovedChunks = m_RemovedShadowChunks
                }.Schedule(JobHandle.CombineDependencies(changesJobHandle, allocateNewShadowChunksJobHandle));

                var prepareResultJob = new PrepareResultsJob
                {
                    GatheredChanges = m_GatheredChanges,
                    RemovedShadowChunks = m_RemovedShadowChunks.AsDeferredJobArray(),
                    IndexOfFirstAdded = &indexOfFirstAdded,
                    ShadowChunksBySequenceNumber = m_ShadowChunks,
                    IndicesInManagedComponentStore = indicesInManagedComponentStore,
                    AddedEntities = addedEntities,
                    AddedEntitiesMappingToComponent = addedEntitiesMapping,
                    RemovedEntities = removedEntities,
                    RemovedEntitiesMappingToComponent = removedEntitiesMapping,
                }.Schedule(copyJobHandle);

                var concatResultsJob = new ConcatResultsJob
                {
                    TypeIndex = m_TypeIndex,
                    GatheredChanges = m_GatheredChanges,
                    RemovedShadowChunks = m_RemovedShadowChunks.AsDeferredJobArray(),
                    ShadowChunksBySequenceNumber = m_ShadowChunks,
                    SharedComponentValueIndexByChunk = m_ManagedComponentIndexInCopyByChunk,
                    IndicesInManagedComponentStore = indicesInManagedComponentStore.AsDeferredJobArray(),
                    AddedEntities = addedEntities.AsDeferredJobArray(),
                    AddedEntitiesMappingToComponent = addedEntitiesMapping.AsDeferredJobArray(),
                    RemovedEntities = removedEntities.AsDeferredJobArray(),
                    RemovedEntitiesMappingToComponent = removedEntitiesMapping.AsDeferredJobArray()
                }.Schedule(prepareResultJob);
                k_GatherComponentChangesJobWorkScheduling.End();

                k_GatherComponentChangesJobWorkCompleting.Begin();
                concatResultsJob.Complete();
                k_GatherComponentChangesJobWorkCompleting.End();

                k_GatherComponentChangesResultProcessing.Begin();
                sharedComponentDataBuffer.Capacity = indicesInManagedComponentStore.Length;
                for (var i = 0; i < indexOfFirstAdded; i++)
                {
                    sharedComponentDataBuffer.AddNoResize(GCHandle.Alloc(m_ManagedComponentStoreStateCopy[indicesInManagedComponentStore[i]]));
                }

                var count = entityManager.GetCheckedEntityDataAccess()->ManagedComponentStore.GetSharedComponentCount();
                m_ManagedComponentStoreStateCopy.Capacity = count;
                m_ManagedComponentStoreStateCopy.Clear();

                // Add the default component value in position 0
                // and query GetSharedComponentData *NonDefault* Boxed to avoid calling Activator.CreateInstance for the default value.
                // A downside is the default shared component value is reused between runs and can be mutated by user code.
                // Can be prevented by adding a check like TypeManager.Equals(m_DefaultComponentDataValue, default(T)) but that would be more expensive.
                m_ManagedComponentStoreStateCopy.Add(m_DefaultComponentDataValue);
                for (var i = 1; i < count; i++)
                {
                    var sharedComponentDataNonDefaultBoxed = entityManager.GetCheckedEntityDataAccess()->ManagedComponentStore.GetSharedComponentDataNonDefaultBoxed(i);
                    m_ManagedComponentStoreStateCopy.Add(sharedComponentDataNonDefaultBoxed);
                }

                for (var i = indexOfFirstAdded; i < indicesInManagedComponentStore.Length; i++)
                {
                    sharedComponentDataBuffer.AddNoResize(GCHandle.Alloc(m_ManagedComponentStoreStateCopy[indicesInManagedComponentStore[i]]));
                }
                k_GatherComponentChangesResultProcessing.End();

                k_GatherComponentChangesResultBufferDispose.Begin();
                for (var i = 0; i < m_GatheredChanges.Length; i++)
                {
                    var c = m_GatheredChanges[i];
                    c.Dispose();
                }

                chunks.Dispose();
                indicesInManagedComponentStore.Dispose();
                k_GatherComponentChangesResultBufferDispose.End();

                return new ComponentChanges(m_TypeIndex, sharedComponentDataBuffer, addedEntities, addedEntitiesMapping, removedEntities, removedEntitiesMapping);
            }
        }

        [BurstCompile]
        unsafe struct GatherChangesJob : IJobParallelFor
        {
            public int TypeIndex;

            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [ReadOnly] public NativeHashMap<ulong, ShadowChunk> ShadowChunksBySequenceNumber;
            [NativeDisableUnsafePtrRestriction] public ChangesCollector* GatheredChanges;

            public void Execute(int index)
            {
                var chunk = Chunks[index].m_Chunk;
                var archetype = chunk->Archetype;
                var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(archetype, TypeIndex);
                if (indexInTypeArray == -1) // Archetype doesn't match required component
                    return;

                var changesForChunk = GatheredChanges + index;

                if (ShadowChunksBySequenceNumber.TryGetValue(chunk->SequenceNumber, out var shadow))
                {
                    if (!ChangeVersionUtility.DidChange(chunk->GetChangeVersion(0), shadow.Version))
                        return;

                    if (!changesForChunk->AddedEntities.IsCreated)
                    {
                        changesForChunk->Chunk = chunk;
                        changesForChunk->AddedEntities = new UnsafeList(Allocator.TempJob);
                        changesForChunk->RemovedEntities = new UnsafeList(Allocator.TempJob);
                    }

                    var entityDataPtr = (Entity*)(chunk->Buffer + archetype->Offsets[0]);
                    var currentCount = chunk->Count;
                    var previousCount = shadow.EntityCount;
                    var i = 0;
                    for (; i < currentCount && i < previousCount; i++)
                    {
                        var currentEntity = entityDataPtr[i];
                        var previousEntity = shadow.EntityDataBuffer[i];

                        if (currentEntity != previousEntity)
                        {
                            // CHANGED ENTITY!
                            changesForChunk->RemovedEntities.Add(previousEntity);
                            changesForChunk->AddedEntities.Add(currentEntity);
                        }
                    }

                    for (; i < currentCount; i++)
                    {
                        // NEW ENTITY!
                        changesForChunk->AddedEntities.Add(entityDataPtr[i]);
                    }

                    for (; i < previousCount; i++)
                    {
                        // REMOVED ENTITY!
                        changesForChunk->RemovedEntities.Add(shadow.EntityDataBuffer[i]);
                    }
                }
                else
                {
                    // This is a new chunk
                    var addedEntities = new UnsafeList(sizeof(Entity), 4, chunk->Count, Allocator.TempJob);
                    var entityDataPtr = chunk->Buffer + archetype->Offsets[0];
                    addedEntities.AddRange<Entity>(entityDataPtr, chunk->Count);
                    changesForChunk->Chunk = chunk;
                    changesForChunk->AddedEntities = addedEntities;
                }
            }
        }

        [BurstCompile]
        unsafe struct AllocateNewShadowChunksJob : IJobParallelFor
        {
            public int TypeIndex;
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [ReadOnly] public NativeHashMap<ulong, ShadowChunk> ShadowChunksBySequenceNumber;
            [NativeDisableUnsafePtrRestriction] public ShadowChunk* AllocatedShadowChunks;

            public void Execute(int index)
            {
                var chunk = Chunks[index].m_Chunk;
                var archetype = chunk->Archetype;
                var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(archetype, TypeIndex);
                if (indexInTypeArray == -1) // Archetype doesn't match required component
                    return;

                var sequenceNumber = chunk->SequenceNumber;
                if (ShadowChunksBySequenceNumber.TryGetValue(sequenceNumber, out var shadow))
                    return;

                var entityDataPtr = chunk->Buffer + archetype->Offsets[0];

                shadow = new ShadowChunk
                {
                    EntityCount = chunk->Count,
                    Version = chunk->GetChangeVersion(0),
                    EntityDataBuffer = (Entity*)UnsafeUtility.Malloc(sizeof(Entity) * chunk->Capacity, 4, Allocator.Persistent),
                };

                UnsafeUtility.MemCpy(shadow.EntityDataBuffer, entityDataPtr, chunk->Count * sizeof(Entity));

                AllocatedShadowChunks[index] = shadow;
            }
        }

        [BurstCompile]
        unsafe struct CopyStateToShadowChunksJob : IJob
        {
            public int TypeIndex;

            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [ReadOnly, NativeDisableUnsafePtrRestriction] public ShadowChunk* AllocatedShadowChunks;
            public NativeHashMap<ulong, ShadowChunk> ShadowChunksBySequenceNumber;
            [WriteOnly] public NativeList<ulong> RemovedChunks;

            public void Execute()
            {
                var knownChunks = ShadowChunksBySequenceNumber.GetKeyArray(Allocator.Temp);
                var processedChunks = new NativeHashMap<ulong, byte>(Chunks.Length, Allocator.Temp);
                for (var index = 0; index < Chunks.Length; index++)
                {
                    var chunk = Chunks[index].m_Chunk;
                    var archetype = chunk->Archetype;
                    var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(archetype, TypeIndex);
                    if (indexInTypeArray == -1) // Archetype doesn't match required component
                        continue;

                    var version = chunk->GetChangeVersion(0);
                    var sequenceNumber = chunk->SequenceNumber;
                    processedChunks[sequenceNumber] = 0;
                    var entityDataPtr = chunk->Buffer + archetype->Offsets[0];

                    if (ShadowChunksBySequenceNumber.TryGetValue(sequenceNumber, out var shadow))
                    {
                        if (!ChangeVersionUtility.DidChange(version, shadow.Version))
                            continue;

                        UnsafeUtility.MemCpy(shadow.EntityDataBuffer, entityDataPtr, chunk->Count * sizeof(Entity));

                        shadow.EntityCount = chunk->Count;
                        shadow.Version = version;

                        ShadowChunksBySequenceNumber[sequenceNumber] = shadow;
                    }
                    else
                    {
                        ShadowChunksBySequenceNumber.Add(sequenceNumber, *(AllocatedShadowChunks + index));
                    }
                }

                for (var i = 0; i < knownChunks.Length; i++)
                {
                    var chunkSequenceNumber = knownChunks[i];
                    if (!processedChunks.ContainsKey(chunkSequenceNumber))
                    {
                        // This is a missing chunk
                        RemovedChunks.Add(chunkSequenceNumber);
                    }
                }

                knownChunks.Dispose();
                processedChunks.Dispose();
            }
        }

        [BurstCompile]
        unsafe struct PrepareResultsJob : IJob
        {
            [ReadOnly] public NativeArray<ChangesCollector> GatheredChanges;
            [ReadOnly] public NativeArray<ulong> RemovedShadowChunks;

            [NativeDisableUnsafePtrRestriction] public int* IndexOfFirstAdded;
            [ReadOnly] public NativeHashMap<ulong, ShadowChunk> ShadowChunksBySequenceNumber;

            public NativeList<int> IndicesInManagedComponentStore;
            public NativeList<Entity> AddedEntities;
            public NativeList<int> AddedEntitiesMappingToComponent;
            public NativeList<Entity> RemovedEntities;
            public NativeList<int> RemovedEntitiesMappingToComponent;

            public void Execute()
            {
                var addedEntityCount = 0;
                var removedEntityCount = 0;
                var addedChunkCount = 0;
                var removedChunkCount = RemovedShadowChunks.Length;
                for (var i = 0; i < RemovedShadowChunks.Length; i++)
                {
                    removedEntityCount += ShadowChunksBySequenceNumber[RemovedShadowChunks[i]].EntityCount;
                }

                for (var i = 0; i < GatheredChanges.Length; i++)
                {
                    var addedEntitiesCount = GatheredChanges[i].AddedEntities.Length;
                    var removedEntitiesCount = GatheredChanges[i].RemovedEntities.Length;
                    addedEntityCount += addedEntitiesCount;
                    removedEntityCount += removedEntitiesCount;
                    if (addedEntitiesCount > 0)
                        addedChunkCount++;
                    if (removedEntitiesCount > 0)
                        removedChunkCount++;
                }

                IndexOfFirstAdded[0] = removedChunkCount;
                IndicesInManagedComponentStore.ResizeUninitialized(addedChunkCount + removedChunkCount);
                AddedEntities.ResizeUninitialized(addedEntityCount);
                AddedEntitiesMappingToComponent.ResizeUninitialized(addedEntityCount);
                RemovedEntities.ResizeUninitialized(removedEntityCount);
                RemovedEntitiesMappingToComponent.ResizeUninitialized(removedEntityCount);
            }
        }

        [BurstCompile]
        unsafe struct ConcatResultsJob : IJob
        {
            public int TypeIndex;

            [ReadOnly] public NativeArray<ChangesCollector> GatheredChanges;
            [ReadOnly] public NativeArray<ulong> RemovedShadowChunks;

            public NativeHashMap<ulong, ShadowChunk> ShadowChunksBySequenceNumber;
            public NativeHashMap<ulong, int> SharedComponentValueIndexByChunk;

            [WriteOnly] public NativeArray<int> IndicesInManagedComponentStore;
            [WriteOnly] public NativeArray<Entity> AddedEntities;
            [WriteOnly] public NativeArray<int> AddedEntitiesMappingToComponent;
            [WriteOnly] public NativeArray<Entity> RemovedEntities;
            [WriteOnly] public NativeArray<int> RemovedEntitiesMappingToComponent;

            public void Execute()
            {
                var addedSharedComponentsCount = 0;
                var removedSharedComponentsCount = 0;
                var removedEntityCurrentCount = 0;
                var addedEntityCurrentCount = 0;

                for (var i = 0; i < RemovedShadowChunks.Length; i++)
                {
                    var chunkSequenceNumber = RemovedShadowChunks[i];
                    var shadowChunk = ShadowChunksBySequenceNumber[chunkSequenceNumber];

                    UnsafeUtility.MemCpy((Entity*)RemovedEntities.GetUnsafePtr() + removedEntityCurrentCount, shadowChunk.EntityDataBuffer, shadowChunk.EntityCount * sizeof(Entity));
                    UnsafeUtility.MemCpyReplicate((int*)RemovedEntitiesMappingToComponent.GetUnsafePtr() + removedEntityCurrentCount, &removedSharedComponentsCount, sizeof(int), shadowChunk.EntityCount);
                    removedEntityCurrentCount += shadowChunk.EntityCount;

                    IndicesInManagedComponentStore[removedSharedComponentsCount++] = SharedComponentValueIndexByChunk[chunkSequenceNumber];

                    ShadowChunksBySequenceNumber.Remove(chunkSequenceNumber);
                    SharedComponentValueIndexByChunk.Remove(chunkSequenceNumber);
                    UnsafeUtility.Free(shadowChunk.EntityDataBuffer, Allocator.Persistent);
                }

                for (var i = 0; i < GatheredChanges.Length; i++)
                {
                    var changes = GatheredChanges[i];
                    if (changes.RemovedEntities.Length == 0)
                        continue;

                    UnsafeUtility.MemCpy((Entity*)RemovedEntities.GetUnsafePtr() + removedEntityCurrentCount, changes.RemovedEntities.Ptr, changes.RemovedEntities.Length * sizeof(Entity));
                    UnsafeUtility.MemCpyReplicate((int*)RemovedEntitiesMappingToComponent.GetUnsafePtr() + removedEntityCurrentCount, &removedSharedComponentsCount, sizeof(int), changes.RemovedEntities.Length);
                    removedEntityCurrentCount += changes.RemovedEntities.Length;

                    IndicesInManagedComponentStore[removedSharedComponentsCount++] = SharedComponentValueIndexByChunk[changes.Chunk->SequenceNumber];
                }

                for (var i = 0; i < GatheredChanges.Length; i++)
                {
                    var changes = GatheredChanges[i];
                    if (changes.AddedEntities.Length == 0)
                        continue;

                    var chunkSequenceNumber = changes.Chunk->SequenceNumber;

                    if (changes.AddedEntities.Length > 0)
                    {
                        var archetype = changes.Chunk->Archetype;
                        var indexInTypeArray = ChunkDataUtility.GetIndexInTypeArray(archetype, TypeIndex);
                        var sharedComponentValueArray = changes.Chunk->SharedComponentValues;
                        var sharedComponentOffset = indexInTypeArray - archetype->FirstSharedComponent;
                        var sharedComponentDataIndex = sharedComponentValueArray[sharedComponentOffset];

                        SharedComponentValueIndexByChunk[chunkSequenceNumber] = sharedComponentDataIndex;

                        UnsafeUtility.MemCpy((Entity*)AddedEntities.GetUnsafePtr() + addedEntityCurrentCount, changes.AddedEntities.Ptr, changes.AddedEntities.Length * sizeof(Entity));
                        var index = removedSharedComponentsCount + addedSharedComponentsCount;
                        UnsafeUtility.MemCpyReplicate((int*)AddedEntitiesMappingToComponent.GetUnsafePtr() + addedEntityCurrentCount, &index, sizeof(int), changes.AddedEntities.Length);
                        addedEntityCurrentCount += changes.AddedEntities.Length;

                        IndicesInManagedComponentStore[index] = sharedComponentDataIndex;
                        addedSharedComponentsCount++;
                    }
                }
            }
        }

        unsafe struct ChangesCollector : IDisposable
        {
            public Chunk* Chunk;
            public UnsafeList AddedEntities;
            public UnsafeList RemovedEntities;

            public void Dispose()
            {
                Chunk = null;
                if (AddedEntities.IsCreated)
                    AddedEntities.Dispose();
                if (RemovedEntities.IsCreated)
                    RemovedEntities.Dispose();
            }
        }

        internal readonly struct ComponentChanges : IDisposable
        {
            readonly int m_ComponentTypeIndex;
            readonly NativeList<GCHandle> m_Buffer;
            readonly NativeList<Entity> m_AddedEntities;
            readonly NativeList<int> m_AddedEntitiesMapping;
            readonly NativeList<Entity> m_RemovedEntities;
            readonly NativeList<int> m_RemovedEntitiesMapping;

            public ComponentChanges(int componentTypeIndex,
                                    NativeList<GCHandle> buffer,
                                    NativeList<Entity> addedEntities,
                                    NativeList<int> addedEntitiesMapping,
                                    NativeList<Entity> removedEntities,
                                    NativeList<int> removedEntitiesMapping)
            {
                m_ComponentTypeIndex = componentTypeIndex;
                m_Buffer = buffer;
                m_AddedEntities = addedEntities;
                m_AddedEntitiesMapping = addedEntitiesMapping;
                m_RemovedEntities = removedEntities;
                m_RemovedEntitiesMapping = removedEntitiesMapping;
            }

            public int AddedEntitiesCount => m_AddedEntities.Length;
            public int RemovedEntitiesCount => m_RemovedEntities.Length;

            public Entity GetAddedEntity(int index) => m_AddedEntities[index];
            public Entity GetRemovedEntity(int index) => m_RemovedEntities[index];

            public T GetAddedComponent<T>(int index) where T : struct, ISharedComponentData
            {
                EnsureIsExpectedComponent<T>();
                return (T)m_Buffer[m_AddedEntitiesMapping[index]].Target;
            }

            public T GetRemovedComponent<T>(int index) where T : struct, ISharedComponentData
            {
                EnsureIsExpectedComponent<T>();
                return (T)m_Buffer[m_RemovedEntitiesMapping[index]].Target;
            }

            void EnsureIsExpectedComponent<T>() where T : struct
            {
                if (TypeManager.GetTypeIndex<T>() != m_ComponentTypeIndex)
                    throw new InvalidOperationException($"Unable to retrieve data for component type {typeof(T)} (type index {TypeManager.GetTypeIndex<T>()}), this container only holds data for the type with type index {m_ComponentTypeIndex}.");
            }

            public void Dispose()
            {
                for (var i = 0; i < m_Buffer.Length; i++)
                {
                    m_Buffer[i].Free();
                }

                m_Buffer.Dispose();
                m_AddedEntities.Dispose();
                m_AddedEntitiesMapping.Dispose();
                m_RemovedEntities.Dispose();
                m_RemovedEntitiesMapping.Dispose();
            }
        }
    }
}

using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;

namespace Unity.Entities.Editor
{
    class ComponentDataDiffer : IDisposable
    {
        static readonly ProfilerMarker k_GatherComponentChangesAsync = new ProfilerMarker($"{nameof(ComponentDataDiffer)}.{nameof(GatherComponentChangesAsync)}");
        static readonly ProfilerMarker k_GatherComponentChangesAsyncBufferAlloc = new ProfilerMarker($"{nameof(ComponentDataDiffer)}.{nameof(GatherComponentChangesAsync)} buffer mgmt");
        static readonly ProfilerMarker k_GatherComponentChangesAsyncScheduling = new ProfilerMarker($"{nameof(ComponentDataDiffer)}.{nameof(GatherComponentChangesAsync)} scheduling");

        readonly int m_TypeIndex;
        readonly int m_ComponentSize;
        readonly NativeHashMap<ulong, ShadowChunk> m_PreviousChunksBySequenceNumber;

        NativeList<ShadowChunk> m_AllocatedShadowChunksForTheFrame;
        NativeList<ChangesCollector> m_GatheredChanges;
        NativeList<byte> m_RemovedChunkBuffer;
        NativeList<Entity> m_RemovedChunkEntities;

        public ComponentDataDiffer(ComponentType componentType)
        {
            if (!CanWatch(componentType))
                throw new ArgumentException($"{nameof(ComponentDataDiffer)} only supports unmanaged {nameof(IComponentData)} components.", nameof(componentType));

            var typeInfo = TypeManager.GetTypeInfo(componentType.TypeIndex);

            WatchedComponentType = componentType;
            m_TypeIndex = typeInfo.TypeIndex;
            m_ComponentSize = typeInfo.SizeInChunk;
            m_PreviousChunksBySequenceNumber = new NativeHashMap<ulong, ShadowChunk>(16, Allocator.Persistent);

            m_AllocatedShadowChunksForTheFrame = new NativeList<ShadowChunk>(16, Allocator.Persistent);
            m_GatheredChanges = new NativeList<ChangesCollector>(16, Allocator.Persistent);
            m_RemovedChunkBuffer = new NativeList<byte>(Allocator.Persistent);
            m_RemovedChunkEntities = new NativeList<Entity>(Allocator.Persistent);
        }

        public ComponentType WatchedComponentType { get; }

        public static bool CanWatch(ComponentType componentType)
        {
            var typeInfo = TypeManager.GetTypeInfo(componentType.TypeIndex);
            return typeInfo.Category == TypeManager.TypeCategory.ComponentData && UnsafeUtility.IsUnmanaged(componentType.GetManagedType());
        }

        public void Dispose()
        {
            unsafe
            {
                using (var array = m_PreviousChunksBySequenceNumber.GetValueArray(Allocator.Temp))
                {
                    for (var i = 0; i < array.Length; i++)
                    {
                        UnsafeUtility.Free(array[i].EntityDataBuffer, Allocator.Persistent);
                        UnsafeUtility.Free(array[i].ComponentDataBuffer, Allocator.Persistent);
                    }
                }
            }

            m_PreviousChunksBySequenceNumber.Dispose();
            m_AllocatedShadowChunksForTheFrame.Dispose();
            m_GatheredChanges.Dispose();
            m_RemovedChunkBuffer.Dispose();
            m_RemovedChunkEntities.Dispose();
        }

        public unsafe ComponentChanges GatherComponentChangesAsync(EntityQuery query, Allocator allocator, out JobHandle jobHandle)
        {
            using (k_GatherComponentChangesAsync.Auto())
            {
                var chunks = query.CreateArchetypeChunkArrayAsync(Allocator.TempJob, out var chunksJobHandle);

                k_GatherComponentChangesAsyncBufferAlloc.Begin();
                m_AllocatedShadowChunksForTheFrame.Clear();
                m_AllocatedShadowChunksForTheFrame.ResizeUninitialized(chunks.Length);
                m_GatheredChanges.Clear();
                m_GatheredChanges.Resize(chunks.Length, NativeArrayOptions.ClearMemory);
                m_RemovedChunkBuffer.Clear();
                m_RemovedChunkEntities.Clear();
                var result = new NativeReference<Result>(allocator);
                k_GatherComponentChangesAsyncBufferAlloc.End();

                k_GatherComponentChangesAsyncScheduling.Begin();
                var changesJobHandle = new GatherComponentChangesJob
                {
                    TypeIndex = m_TypeIndex,
                    ComponentSize = m_ComponentSize,
                    Chunks = chunks,
                    ShadowChunksBySequenceNumber = m_PreviousChunksBySequenceNumber,
                    GatheredChanges = (ChangesCollector*)m_GatheredChanges.GetUnsafeList()->Ptr
                }.Schedule(chunks.Length, 1, chunksJobHandle);

                var allocateNewShadowChunksJobHandle = new AllocateNewShadowChunksJob
                {
                    TypeIndex = m_TypeIndex,
                    ComponentSize = m_ComponentSize,
                    Chunks = chunks,
                    ShadowChunksBySequenceNumber = m_PreviousChunksBySequenceNumber,
                    AllocatedShadowChunks = (ShadowChunk*)m_AllocatedShadowChunksForTheFrame.GetUnsafeList()->Ptr
                }.Schedule(chunks.Length, 1, chunksJobHandle);

                var copyJobHandle = new CopyComponentDataJob
                {
                    TypeIndex = m_TypeIndex,
                    ComponentSize = m_ComponentSize,
                    Chunks = chunks,
                    ShadowChunksBySequenceNumber = m_PreviousChunksBySequenceNumber,
                    AllocatedShadowChunks = (ShadowChunk*)m_AllocatedShadowChunksForTheFrame.GetUnsafeList()->Ptr,
                    RemovedChunkComponentDataBuffer = m_RemovedChunkBuffer,
                    RemovedChunkEntities = m_RemovedChunkEntities
                }.Schedule(JobHandle.CombineDependencies(changesJobHandle, allocateNewShadowChunksJobHandle));

                var concatResultJobHandle = new ConcatResultJob
                {
                    ComponentSize = m_ComponentSize,
                    Allocator = allocator,
                    GatheredChanges = m_GatheredChanges.AsDeferredJobArray(),
                    RemovedChunkComponentDataBuffer = m_RemovedChunkBuffer.AsDeferredJobArray(),
                    RemovedChunkEntities = m_RemovedChunkEntities.AsDeferredJobArray(),

                    Result = result
                }.Schedule(copyJobHandle);

                jobHandle = JobHandle.CombineDependencies(chunks.Dispose(copyJobHandle), concatResultJobHandle);
                k_GatherComponentChangesAsyncScheduling.End();

                return new ComponentChanges(m_TypeIndex, result);
            }
        }

        [BurstCompile]
        unsafe struct GatherComponentChangesJob : IJobParallelFor
        {
            public int TypeIndex;
            public int ComponentSize;

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
                    if (!ChangeVersionUtility.DidChange(chunk->GetChangeVersion(indexInTypeArray), shadow.ComponentVersion)
                        && !ChangeVersionUtility.DidChange(chunk->GetChangeVersion(0), shadow.EntityVersion))
                        return;

                    if (!changesForChunk->AddedComponentEntities.IsCreated)
                    {
                        changesForChunk->AddedComponentEntities = new UnsafeList(Allocator.TempJob);
                        changesForChunk->AddedComponentDataBuffer = new UnsafeList(Allocator.TempJob);
                        changesForChunk->RemovedComponentEntities = new UnsafeList(Allocator.TempJob);
                        changesForChunk->RemovedComponentDataBuffer = new UnsafeList(Allocator.TempJob);
                    }

                    var entityDataPtr = chunk->Buffer + archetype->Offsets[0];
                    var componentDataPtr = chunk->Buffer + archetype->Offsets[indexInTypeArray];

                    var currentCount = chunk->Count;
                    var previousCount = shadow.Count;

                    var i = 0;

                    for (; i < currentCount && i < previousCount; i++)
                    {
                        var currentComponentData = componentDataPtr + ComponentSize * i;
                        var previousComponentData = shadow.ComponentDataBuffer + ComponentSize * i;

                        var entity = *(Entity*)(entityDataPtr + sizeof(Entity) * i);
                        var previousEntity = *(Entity*)(shadow.EntityDataBuffer + sizeof(Entity) * i);

                        if (entity != previousEntity
                            || UnsafeUtility.MemCmp(currentComponentData, previousComponentData, ComponentSize) != 0)
                        {
                            // CHANGED COMPONENT DATA!
                            OnRemovedComponent(changesForChunk, previousEntity, previousComponentData, ComponentSize);
                            OnNewComponent(changesForChunk, entity, currentComponentData, ComponentSize);
                        }
                    }

                    for (; i < currentCount; i++)
                    {
                        // NEW COMPONENT DATA!
                        var entity = *(Entity*)(entityDataPtr + sizeof(Entity) * i);
                        var currentComponentData = componentDataPtr + ComponentSize * i;
                        OnNewComponent(changesForChunk, entity, currentComponentData, ComponentSize);
                    }

                    for (; i < previousCount; i++)
                    {
                        // REMOVED COMPONENT DATA!
                        var entity = *(Entity*)(entityDataPtr + sizeof(Entity) * i);
                        var previousComponentData = shadow.ComponentDataBuffer + ComponentSize * i;
                        OnRemovedComponent(changesForChunk, entity, previousComponentData, ComponentSize);
                    }
                }
                else
                {
                    // This is a new chunk
                    var addedComponentDataBuffer = new UnsafeList(ComponentSize, 4, chunk->Count, Allocator.TempJob);
                    var addedComponentEntities = new UnsafeList(sizeof(Entity), 4, chunk->Count, Allocator.TempJob);

                    var entityDataPtr = chunk->Buffer + archetype->Offsets[0];
                    var componentDataPtr = chunk->Buffer + archetype->Offsets[indexInTypeArray];

                    addedComponentDataBuffer.AddRange<byte>(componentDataPtr, chunk->Count * ComponentSize);
                    addedComponentEntities.AddRange<Entity>(entityDataPtr, chunk->Count);

                    changesForChunk->AddedComponentDataBuffer = addedComponentDataBuffer;
                    changesForChunk->AddedComponentEntities = addedComponentEntities;
                }
            }

            static void OnNewComponent(ChangesCollector* changesForChunk, Entity entity, byte* currentComponentData, int componentSize)
            {
                changesForChunk->AddedComponentEntities.Add(entity);
                changesForChunk->AddedComponentDataBuffer.AddRange<byte>(currentComponentData, componentSize);
            }

            static void OnRemovedComponent(ChangesCollector* changesForChunk, Entity entity, byte* previousComponentData, int componentSize)
            {
                changesForChunk->RemovedComponentEntities.Add(entity);
                changesForChunk->RemovedComponentDataBuffer.AddRange<byte>(previousComponentData, componentSize);
            }
        }

        [BurstCompile]
        unsafe struct AllocateNewShadowChunksJob : IJobParallelFor
        {
            public int TypeIndex;
            public int ComponentSize;
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
                var componentDataPtr = chunk->Buffer + archetype->Offsets[indexInTypeArray];

                shadow = new ShadowChunk
                {
                    Count = chunk->Count,
                    ComponentVersion = chunk->GetChangeVersion(indexInTypeArray),
                    EntityVersion = chunk->GetChangeVersion(0),
                    EntityDataBuffer = (byte*)UnsafeUtility.Malloc(sizeof(Entity) * chunk->Capacity, 4, Allocator.Persistent),
                    ComponentDataBuffer = (byte*)UnsafeUtility.Malloc(ComponentSize * chunk->Capacity, 4, Allocator.Persistent)
                };

                UnsafeUtility.MemCpy(shadow.EntityDataBuffer, entityDataPtr, chunk->Count * sizeof(Entity));
                UnsafeUtility.MemCpy(shadow.ComponentDataBuffer, componentDataPtr, chunk->Count * ComponentSize);

                AllocatedShadowChunks[index] = shadow;
            }
        }

        [BurstCompile]
        unsafe struct CopyComponentDataJob : IJob
        {
            public int TypeIndex;
            public int ComponentSize;

            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [ReadOnly, NativeDisableUnsafePtrRestriction] public ShadowChunk* AllocatedShadowChunks;
            public NativeHashMap<ulong, ShadowChunk> ShadowChunksBySequenceNumber;
            [WriteOnly] public NativeList<byte> RemovedChunkComponentDataBuffer;
            [WriteOnly] public NativeList<Entity> RemovedChunkEntities;

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

                    var componentVersion = chunk->GetChangeVersion(indexInTypeArray);
                    var entityVersion = chunk->GetChangeVersion(0);
                    var sequenceNumber = chunk->SequenceNumber;
                    processedChunks[sequenceNumber] = 0;
                    var entityDataPtr = chunk->Buffer + archetype->Offsets[0];
                    var componentDataPtr = chunk->Buffer + archetype->Offsets[indexInTypeArray];

                    if (ShadowChunksBySequenceNumber.TryGetValue(sequenceNumber, out var shadow))
                    {
                        if (!ChangeVersionUtility.DidChange(componentVersion, shadow.ComponentVersion)
                            && !ChangeVersionUtility.DidChange(entityVersion, shadow.EntityVersion))
                            continue;

                        UnsafeUtility.MemCpy(shadow.EntityDataBuffer, entityDataPtr, chunk->Count * sizeof(Entity));
                        UnsafeUtility.MemCpy(shadow.ComponentDataBuffer, componentDataPtr, chunk->Count * ComponentSize);

                        shadow.Count = chunk->Count;
                        shadow.ComponentVersion = componentVersion;
                        shadow.EntityVersion = entityVersion;

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
                        var shadowChunk = ShadowChunksBySequenceNumber[chunkSequenceNumber];

                        // REMOVED COMPONENT DATA!
                        RemovedChunkComponentDataBuffer.AddRange(shadowChunk.ComponentDataBuffer, shadowChunk.Count * ComponentSize);
                        RemovedChunkEntities.AddRange(shadowChunk.EntityDataBuffer, shadowChunk.Count);

                        UnsafeUtility.Free(shadowChunk.ComponentDataBuffer, Allocator.Persistent);
                        UnsafeUtility.Free(shadowChunk.EntityDataBuffer, Allocator.Persistent);

                        ShadowChunksBySequenceNumber.Remove(chunkSequenceNumber);
                    }
                }

                knownChunks.Dispose();
                processedChunks.Dispose();
            }
        }

        [BurstCompile]
        unsafe struct ConcatResultJob : IJob
        {
            public int ComponentSize;
            public Allocator Allocator;
            [ReadOnly] public NativeArray<ChangesCollector> GatheredChanges;
            [ReadOnly] public NativeArray<byte> RemovedChunkComponentDataBuffer;
            [ReadOnly] public NativeArray<Entity> RemovedChunkEntities;

            [WriteOnly] public NativeReference<Result> Result;

            public void Execute()
            {
                var addedEntityCount = 0;
                var removedEntityCount = RemovedChunkEntities.Length;
                for (var i = 0; i < GatheredChanges.Length; i++)
                {
                    var changesForChunk = GatheredChanges[i];
                    addedEntityCount += changesForChunk.AddedComponentEntities.Length;
                    removedEntityCount += changesForChunk.RemovedComponentEntities.Length;
                }

                if (addedEntityCount == 0 && removedEntityCount == 0)
                    return;

                var buffer = new UnsafeList(UnsafeUtility.SizeOf<byte>(), UnsafeUtility.AlignOf<byte>(), (addedEntityCount + removedEntityCount) * ComponentSize, Allocator);
                var addedComponents = new UnsafeList(UnsafeUtility.SizeOf<Entity>(), UnsafeUtility.AlignOf<Entity>(), addedEntityCount, Allocator);
                var removedComponents = new UnsafeList(UnsafeUtility.SizeOf<Entity>(), UnsafeUtility.AlignOf<Entity>(), removedEntityCount, Allocator);

                var chunksWithRemovedData = new NativeList<int>(GatheredChanges.Length, Allocator.Temp);
                for (var i = 0; i < GatheredChanges.Length; i++)
                {
                    var changesForChunk = GatheredChanges[i];
                    if (changesForChunk.AddedComponentDataBuffer.IsCreated)
                    {
                        buffer.AddRangeNoResize<byte>(changesForChunk.AddedComponentDataBuffer.Ptr, changesForChunk.AddedComponentDataBuffer.Length);
                        addedComponents.AddRangeNoResize<Entity>(changesForChunk.AddedComponentEntities.Ptr, changesForChunk.AddedComponentEntities.Length);

                        changesForChunk.AddedComponentDataBuffer.Dispose();
                        changesForChunk.AddedComponentEntities.Dispose();
                    }

                    if (changesForChunk.RemovedComponentDataBuffer.IsCreated)
                        chunksWithRemovedData.AddNoResize(i);
                }

                for (var i = 0; i < chunksWithRemovedData.Length; i++)
                {
                    var changesForChunk = GatheredChanges[chunksWithRemovedData[i]];
                    buffer.AddRangeNoResize<byte>(changesForChunk.RemovedComponentDataBuffer.Ptr, changesForChunk.RemovedComponentDataBuffer.Length);
                    removedComponents.AddRangeNoResize<Entity>(changesForChunk.RemovedComponentEntities.Ptr, changesForChunk.RemovedComponentEntities.Length);

                    changesForChunk.RemovedComponentDataBuffer.Dispose();
                    changesForChunk.RemovedComponentEntities.Dispose();
                }

                chunksWithRemovedData.Dispose();

                buffer.AddRangeNoResize<byte>(RemovedChunkComponentDataBuffer.GetUnsafeReadOnlyPtr(), RemovedChunkComponentDataBuffer.Length);
                removedComponents.AddRangeNoResize<Entity>(RemovedChunkEntities.GetUnsafeReadOnlyPtr(), RemovedChunkEntities.Length);

                Result.Value = new Result
                {
                    Buffer = buffer,
                    AddedComponents = addedComponents,
                    RemovedComponents = removedComponents
                };
            }
        }

        unsafe struct ShadowChunk
        {
            public uint ComponentVersion;
            public uint EntityVersion;
            public int Count;
            public byte* EntityDataBuffer;
            public byte* ComponentDataBuffer;
        }

        struct ChangesCollector
        {
            public UnsafeList AddedComponentDataBuffer;
            public UnsafeList RemovedComponentDataBuffer;
            public UnsafeList AddedComponentEntities;
            public UnsafeList RemovedComponentEntities;
        }

        internal struct Result
        {
            public UnsafeList Buffer;
            public UnsafeList AddedComponents;
            public UnsafeList RemovedComponents;
        }

        internal readonly unsafe struct ComponentChanges : IDisposable
        {
            readonly int m_ComponentTypeIndex;
            readonly NativeReference<Result> m_Result;

            public ComponentChanges(int componentTypeIndex,
                                    NativeReference<Result> result)
            {
                m_ComponentTypeIndex = componentTypeIndex;
                m_Result = result;
            }

            public int AddedComponentsCount => m_Result.Value.AddedComponents.IsCreated ? m_Result.Value.AddedComponents.Length : 0;
            public int RemovedComponentsCount => m_Result.Value.RemovedComponents.IsCreated ? m_Result.Value.RemovedComponents.Length : 0;

            public (NativeArray<Entity> entities, NativeArray<T> componentData) GetAddedComponents<T>(Allocator allocator) where T : struct
            {
                EnsureIsExpectedComponent<T>();

                if (!m_Result.Value.Buffer.IsCreated)
                    return (new NativeArray<Entity>(0, allocator), new NativeArray<T>(0, allocator));

                var result = m_Result.Value;
                var addedComponents = result.AddedComponents;
                var entities = new NativeArray<Entity>(addedComponents.Length, allocator);
                var components = new NativeArray<T>(addedComponents.Length, allocator);
                UnsafeUtility.MemCpy(entities.GetUnsafePtr(), addedComponents.Ptr, addedComponents.Length * UnsafeUtility.SizeOf<Entity>());
                UnsafeUtility.MemCpy(components.GetUnsafePtr(), result.Buffer.Ptr, addedComponents.Length * UnsafeUtility.SizeOf<T>());

                return (entities, components);
            }

            public (NativeArray<Entity> entities, NativeArray<T> componentData) GetRemovedComponents<T>(Allocator allocator) where T : struct
            {
                EnsureIsExpectedComponent<T>();

                if (!m_Result.Value.Buffer.IsCreated)
                    return (new NativeArray<Entity>(0, allocator), new NativeArray<T>(0, allocator));

                var result = m_Result.Value;
                var removedComponents = result.RemovedComponents;
                var entities = new NativeArray<Entity>(removedComponents.Length, allocator);
                var components = new NativeArray<T>(removedComponents.Length, allocator);
                UnsafeUtility.MemCpy(entities.GetUnsafePtr(), removedComponents.Ptr, removedComponents.Length * UnsafeUtility.SizeOf<Entity>());
                UnsafeUtility.MemCpy(components.GetUnsafePtr(), (byte*)result.Buffer.Ptr + result.AddedComponents.Length * UnsafeUtility.SizeOf<T>(), removedComponents.Length * UnsafeUtility.SizeOf<T>());

                return (entities, components);
            }

            void EnsureIsExpectedComponent<T>() where T : struct
            {
                if (TypeManager.GetTypeIndex<T>() != m_ComponentTypeIndex)
                    throw new InvalidOperationException($"Unable to retrieve data for component type {typeof(T)} (type index {TypeManager.GetTypeIndex<T>()}), this container only holds data for the type with type index {m_ComponentTypeIndex}.");
            }

            public void Dispose()
            {
                if (m_Result.Value.Buffer.IsCreated)
                {
                    var tempResults = m_Result.Value;
                    tempResults.Buffer.Dispose();
                    tempResults.AddedComponents.Dispose();
                    tempResults.RemovedComponents.Dispose();
                }

                m_Result.Dispose();
            }
        }
    }
}

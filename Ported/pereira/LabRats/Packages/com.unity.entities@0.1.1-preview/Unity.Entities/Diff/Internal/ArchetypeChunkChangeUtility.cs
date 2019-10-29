using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Entities
{
    [System.Flags]
    internal enum ChunkChangeFlags
    {
        None = 0,
        
        /// <summary>
        /// This chunk exists in both worlds. 
        /// </summary>
        Mirrored
    }
    
    internal static class ArchetypeChunkChangeUtility
    {
        [BurstCompile]
        private unsafe struct SetMissingEntityReferencesToNull : IJobParallelFor
        {
            public uint GlobalSystemVersion;
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [ReadOnly] public TypeInfoStream TypeInfoStream;

            [ReadOnly, NativeDisableUnsafePtrRestriction]
            public EntityComponentStore* EntityComponentStore;

            public void Execute(int index)
            {
                var chunk = Chunks[index].m_Chunk;
                var archetype = chunk->Archetype;

                for (var typeIndexInArchetype = 0; typeIndexInArchetype < archetype->TypesCount; typeIndexInArchetype++)
                {
                    var componentTypeInArchetype = archetype->Types[typeIndexInArchetype];
                    var typeInfo = TypeInfoStream.GetTypeInfo(componentTypeInArchetype.TypeIndex);

                    if (typeInfo.EntityOffsetCount == 0)
                    {
                        // This type has no entity references. Skip.
                        continue;
                    }

                    if (componentTypeInArchetype.IsSharedComponent || componentTypeInArchetype.IsZeroSized)
                    {
                        // @TODO Should we handle shared components with references? Is that a thing?
                        continue;
                    }

                    var typeInChunkPtr = ChunkUtility.GetBuffer(chunk) + archetype->Offsets[typeIndexInArchetype];
                    var typeSizeOf = archetype->SizeOfs[typeIndexInArchetype];

                    var changed = false;

                    for (var entityIndexInChunk = 0; entityIndexInChunk < Chunks[index].Count; entityIndexInChunk++)
                    {
                        var componentDataPtr = typeInChunkPtr + typeSizeOf * entityIndexInChunk;

                        if (componentTypeInArchetype.IsBuffer)
                        {
                            var bufferHeader = (BufferHeader*) componentDataPtr;
                            var bufferLength = bufferHeader->Length;
                            var bufferPtr = BufferHeader.GetElementPointer(bufferHeader);
                            changed |= SetMissingEntityReferencesToNullForComponent(typeInfo, bufferPtr, bufferLength);
                        }
                        else
                        {
                            changed |= SetMissingEntityReferencesToNullForComponent(typeInfo, componentDataPtr, 1);
                        }
                    }

                    if (changed)
                    {
                        chunk->SetChangeVersion(typeIndexInArchetype, GlobalSystemVersion);
                    }
                }
            }

            private bool SetMissingEntityReferencesToNullForComponent(
                TypeInfo typeInfo,
                byte* address,
                int elementCount)
            {
                var changed = false;

                for (var elementIndex = 0; elementIndex < elementCount; elementIndex++)
                {
                    var elementPtr = address + typeInfo.ElementSize * elementIndex;

                    for (var entityOffsetIndex = 0; entityOffsetIndex < typeInfo.EntityOffsetCount; entityOffsetIndex++)
                    {
                        var offset = typeInfo.EntityOffsets[entityOffsetIndex];
                        if (EntityComponentStore->Exists(*(Entity*) (elementPtr + offset)))
                        {
                            continue;
                        }

                        *(Entity*) (elementPtr + offset) = Entity.Null;
                        changed = true;
                    }
                }

                return changed;
            }
        }
        
        /// <summary>
        /// Builds a mapping of <see cref="Chunk.SequenceNumber"/> to <see cref="ArchetypeChunk"/>
        /// </summary>
        [BurstCompile]
        private struct BuildChunkSequenceNumberMap : IJobParallelFor
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [WriteOnly] public NativeHashMap<ulong, ArchetypeChunk>.ParallelWriter ChunksBySequenceNumber;

            public unsafe void Execute(int index) => ChunksBySequenceNumber.TryAdd(Chunks[index].m_Chunk->SequenceNumber, Chunks[index]);
        }

        /// <summary>
        /// Builds a set of chunks which have been created or destroyed.
        ///
        /// Created chunks point to the srcWorld while Destroyed chunks point to the dstWorld.
        /// </summary>
        [BurstCompile]
        private unsafe struct BuildArchetypeChunkChanges : IJob
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> SrcChunks;
            [ReadOnly] public NativeArray<ArchetypeChunk> DstChunks;
            [ReadOnly] public NativeHashMap<ulong, ArchetypeChunk> SrcChunksBySequenceNumber;

            public NativeList<ArchetypeChunk> CreatedChunks;
            public NativeList<ChunkChangeFlags> CreatedChunkFlags;
            public NativeArray<int> CreatedChunkOffsets;
            public NativeList<ArchetypeChunk> DestroyedChunks;
            public NativeList<ChunkChangeFlags> DestroyedChunkFlags;
            public NativeArray<int> DestroyedChunkOffsets;
            public NativeHashMap<ulong, byte> VisitedChunks;

            [NativeDisableUnsafePtrRestriction] public int* CreateEntityCount;
            [NativeDisableUnsafePtrRestriction] public int* DestroyedEntityCount;

            public void Execute()
            {
                var createdChunksOffset = 0;
                var destroyedChunksOffset = 0;

                // Scan through the destination chunks.
                for (var i = 0; i < DstChunks.Length; i++)
                {
                    var dstChunk = DstChunks[i];
                    var srcChunk = default(ArchetypeChunk);

                    // Any look for a matching chunk in the destination world.
                    SrcChunksBySequenceNumber.TryGetValue(dstChunk.m_Chunk->SequenceNumber, out srcChunk);

                    if (srcChunk.m_Chunk == null)
                    {
                        // This chunk exists in the destination world but NOT in the source world. 
                        // This means the chunk was simply destroyed.
                        DestroyedChunks.Add(dstChunk);
                        DestroyedChunkFlags.Add(ChunkChangeFlags.None);
                        DestroyedChunkOffsets[DestroyedChunks.Length - 1] = destroyedChunksOffset;
                        destroyedChunksOffset += dstChunk.m_Chunk->Count;
                    }
                    else
                    {
                        if (ChunksAreDifferent(dstChunk.m_Chunk, srcChunk.m_Chunk))
                        {
                            // The chunk exists in both worlds, but it has been changed in some way.
                            // Treat this chunk as being destroyed and re-created.
                            DestroyedChunks.Add(dstChunk);
                            DestroyedChunkFlags.Add(ChunkChangeFlags.Mirrored);
                            DestroyedChunkOffsets[DestroyedChunks.Length - 1] = destroyedChunksOffset;
                            destroyedChunksOffset += dstChunk.m_Chunk->Count;

                            CreatedChunks.Add(srcChunk);
                            CreatedChunkFlags.Add(ChunkChangeFlags.Mirrored);
                            CreatedChunkOffsets[CreatedChunks.Length - 1] = createdChunksOffset;
                            createdChunksOffset += srcChunk.m_Chunk->Count;
                        }

                        VisitedChunks.TryAdd(srcChunk.m_Chunk->SequenceNumber, 1);
                    }
                }

                // Scan through the source chunks.
                for (var i = 0; i < SrcChunks.Length; i++)
                {
                    var srcChunk = SrcChunks[i];

                    // We only care about chunks we have not visited yet.
                    if (!VisitedChunks.TryGetValue(srcChunk.m_Chunk->SequenceNumber, out _))
                    {
                        // This chunk exists in the source world but NOT in the destination world.
                        // This means the chunk was created.
                        CreatedChunks.Add(srcChunk);
                        CreatedChunkFlags.Add(ChunkChangeFlags.None);
                        CreatedChunkOffsets[CreatedChunks.Length - 1] = createdChunksOffset;
                        createdChunksOffset += srcChunk.m_Chunk->Count;
                    }
                }

                *CreateEntityCount = createdChunksOffset;
                *DestroyedEntityCount = destroyedChunksOffset;
            }

            private static bool ChunksAreDifferent(Chunk* srcChunk, Chunk* dstChunk)
            {
                if (srcChunk->Count != dstChunk->Count)
                    return true;

                if (srcChunk->Archetype->TypesCount != dstChunk->Archetype->TypesCount)
                    return true;

                var typeCount = srcChunk->Archetype->TypesCount;

                for (var typeIndex = 0; typeIndex < typeCount; ++typeIndex)
                {
                    if (srcChunk->Archetype->Types[typeIndex] != dstChunk->Archetype->Types[typeIndex])
                        return true;

                    if (srcChunk->GetChangeVersion(typeIndex) != dstChunk->GetChangeVersion(typeIndex))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Clears out any references to non-existent entities.
        /// </summary>
        /// <remarks>
        /// This can potentially mutate the world and increment the changed version of chunks.
        /// </remarks>
        public static unsafe void ClearMissingReferences(
            NativeArray<ArchetypeChunk> chunks,
            EntityComponentStore* entityComponentStore,
            uint globalSystemVersion,
            TypeInfoStream typeInfoStream
        )
        {
            new SetMissingEntityReferencesToNull
            {
                GlobalSystemVersion = globalSystemVersion,
                Chunks = chunks,
                EntityComponentStore = entityComponentStore,
                TypeInfoStream = typeInfoStream
            }.Schedule(chunks.Length, 64).Complete();
        }

        /// <summary>
        /// Builds high level chunk changes between the given chunk sets.
        ///
        /// Created chunks are from the srcWorld while destroyed chunks point to the dstWorld.
        /// </summary>
        /// <param name="srcChunks">A set of chunks from the srcWorld to consider.</param>
        /// <param name="dstChunks">A set of chunks from the dstWorld to consider.</param>
        /// <param name="dstChunkToSrcChunkSequenceNumbers">Mapping of sequence numbers for dstChunk to srcChunk.</param>
        /// <param name="allocator">The allocator that should be used for the returned structure.</param>
        /// <returns>A set of chunks which should have been created and/or destroyed from the srcWorld.</returns>
        public static unsafe ArchetypeChunkChanges GetArchetypeChunkChanges(
            NativeArray<ArchetypeChunk> srcChunks,
            NativeArray<ArchetypeChunk> dstChunks,
            Allocator allocator)
        {
            var createdEntityCount = 0;
            var destroyedEntityCount = 0;

            var createdChunks = new NativeList<ArchetypeChunk>(srcChunks.Length, allocator);
            var destroyedChunks = new NativeList<ArchetypeChunk>(dstChunks.Length, allocator);
            var createdChunkFlags = new NativeList<ChunkChangeFlags>(srcChunks.Length, allocator);
            var destroyedChunkFlags = new NativeList<ChunkChangeFlags>(dstChunks.Length, allocator);
            var createdChunksOffsets = new NativeArray<int>(srcChunks.Length, allocator, NativeArrayOptions.UninitializedMemory);
            var destroyedChunksOffsets = new NativeArray<int>(dstChunks.Length, allocator, NativeArrayOptions.UninitializedMemory);

            using (var srcChunksBySequenceNumber = new NativeHashMap<ulong, ArchetypeChunk>(srcChunks.Length, Allocator.TempJob))
            using (var visitedChunks = new NativeHashMap<ulong, byte>(srcChunks.Length, Allocator.TempJob))
            {
                var buildSrcChunkSequenceNumberMap = new BuildChunkSequenceNumberMap
                {
                    Chunks = srcChunks,
                    ChunksBySequenceNumber = srcChunksBySequenceNumber.AsParallelWriter()
                }.Schedule(srcChunks.Length, 64);

                var buildArchetypeChunkChanges = new BuildArchetypeChunkChanges
                {
                    SrcChunks = srcChunks,
                    DstChunks = dstChunks,
                    SrcChunksBySequenceNumber = srcChunksBySequenceNumber,
                    CreatedChunks = createdChunks,
                    CreatedChunkFlags = createdChunkFlags,
                    CreatedChunkOffsets = createdChunksOffsets,
                    DestroyedChunks = destroyedChunks,
                    DestroyedChunkFlags = destroyedChunkFlags,
                    DestroyedChunkOffsets = destroyedChunksOffsets,
                    VisitedChunks = visitedChunks,
                    CreateEntityCount = &createdEntityCount,
                    DestroyedEntityCount = &destroyedEntityCount
                }.Schedule(buildSrcChunkSequenceNumberMap);

                buildArchetypeChunkChanges.Complete();
            }

            return new ArchetypeChunkChanges(
                new ArchetypeChunkCollection(createdChunks, createdChunkFlags, createdChunksOffsets, createdEntityCount),
                new ArchetypeChunkCollection(destroyedChunks, destroyedChunkFlags, destroyedChunksOffsets, destroyedEntityCount));
        }
    }
}
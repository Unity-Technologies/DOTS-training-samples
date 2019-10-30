using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities
{
    static unsafe partial class EntityDiffer
    {
        [Flags]
        internal enum ArchetypeChunkChangeFlags
        {
            None = 0,
            
            /// <summary>
            /// This chunk exists in both worlds, change version comparisons can be done.
            /// </summary>
            Cloned
        }

        internal readonly struct ArchetypeChunkChanges : IDisposable
        {
            /// <summary>
            /// A set of all chunks in the SrcWorld that have been created.
            /// </summary>
            /// <remarks>
            /// This includes modified chunks that are being re-created.
            /// </remarks>
            public readonly ArchetypeChunkChangeSet CreatedSrcChunks;
            
            /// <summary>
            /// A set of all chunks in the DstWorld that have be destroyed from the SrcWorld.
            /// </summary>
            /// <remarks>
            /// This includes modified chunks that are being re-created.
            /// </remarks>
            public readonly ArchetypeChunkChangeSet DestroyedDstChunks;

            public ArchetypeChunkChanges(Allocator allocator)
            {
                CreatedSrcChunks = new ArchetypeChunkChangeSet(allocator);
                DestroyedDstChunks = new ArchetypeChunkChangeSet(allocator);
            }

            public void Dispose()
            {
                CreatedSrcChunks.Dispose();
                DestroyedDstChunks.Dispose();
            }
        }

        internal struct ArchetypeChunkChangeSet : IDisposable
        {
            /// <summary>
            /// A set of changed chunks within a given world.
            /// </summary>
            public readonly NativeList<ArchetypeChunk> Chunks;
            
            /// <summary>
            /// Change flags for each chunk matched by index.
            /// </summary>
            public readonly NativeList<ArchetypeChunkChangeFlags> Flags;
            
            /// <summary>
            /// An array containing the cumulative count of entities per chunk.
            /// </summary>
            public readonly NativeList<int> EntityCounts;

            /// <summary>
            /// The total number of entities for all changed chunks.
            /// </summary>
            public int TotalEntityCount => EntityCounts.Length > 0 ? EntityCounts[EntityCounts.Length - 1] : 0;

            public ArchetypeChunkChangeSet(Allocator allocator)
            {
                Chunks = new NativeList<ArchetypeChunk>(allocator);
                Flags = new NativeList<ArchetypeChunkChangeFlags>(allocator);
                EntityCounts = new NativeList<int>(allocator);
            }

            public void Dispose()
            {
                Chunks.Dispose();
                Flags.Dispose();
                EntityCounts.Dispose();
            }
        }
        
        /// <summary>
        /// Builds a mapping of <see cref="Chunk.SequenceNumber"/> to it's <see cref="ArchetypeChunk"/>.
        /// </summary>
        [BurstCompile]
        struct BuildChunkSequenceNumberMap : IJobParallelFor
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [WriteOnly] public NativeHashMap<ulong, ArchetypeChunk>.ParallelWriter ChunksBySequenceNumber;
            public void Execute(int index) => ChunksBySequenceNumber.TryAdd(Chunks[index].m_Chunk->SequenceNumber, Chunks[index]);
        }
        
        /// <summary>
        /// Builds a set of chunks which have been created or destroyed.
        ///
        /// Created chunks point to the srcWorld while Destroyed chunks point to the dstWorld.
        /// </summary>
        [BurstCompile]
        struct GatherArchetypeChunkChanges : IJob
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> SrcChunks;
            [ReadOnly] public NativeArray<ArchetypeChunk> DstChunks;
            [ReadOnly] public NativeHashMap<ulong, ArchetypeChunk> SrcChunksBySequenceNumber;
            [WriteOnly] public NativeList<ArchetypeChunk> CreatedChunks;
            [WriteOnly] public NativeList<ArchetypeChunkChangeFlags> CreatedChunkFlags;
            [WriteOnly] public NativeList<int> CreatedChunkEntityCounts;
            [WriteOnly] public NativeList<ArchetypeChunk> DestroyedChunks;
            [WriteOnly] public NativeList<ArchetypeChunkChangeFlags> DestroyedChunkFlags;
            [WriteOnly] public NativeList<int> DestroyedChunkEntityCounts;

            public void Execute()
            {
                var visitedChunks = new NativeHashMap<ulong, int>(1, Allocator.Temp);

                var createdChunkEntityCounts = 0;
                var destroyedChunkEntityCount = 0;

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
                        DestroyedChunkFlags.Add(ArchetypeChunkChangeFlags.None);
                        DestroyedChunkEntityCounts.Add(destroyedChunkEntityCount);
                        destroyedChunkEntityCount += dstChunk.m_Chunk->Count;
                    }
                    else
                    {
                        if (ChunksAreDifferent(dstChunk.m_Chunk, srcChunk.m_Chunk))
                        {
                            // The chunk exists in both worlds, but it has been changed in some way.
                            // Treat this chunk as being destroyed and re-created.
                            DestroyedChunks.Add(dstChunk);
                            DestroyedChunkFlags.Add(ArchetypeChunkChangeFlags.Cloned);
                            DestroyedChunkEntityCounts.Add(destroyedChunkEntityCount);
                            destroyedChunkEntityCount += dstChunk.m_Chunk->Count;

                            CreatedChunks.Add(srcChunk);
                            CreatedChunkFlags.Add(ArchetypeChunkChangeFlags.Cloned);
                            CreatedChunkEntityCounts.Add(createdChunkEntityCounts);
                            createdChunkEntityCounts += srcChunk.m_Chunk->Count;
                        }

                        visitedChunks.TryAdd(srcChunk.m_Chunk->SequenceNumber, 1);
                    }
                }

                // Scan through the source chunks.
                for (var i = 0; i < SrcChunks.Length; i++)
                {
                    var srcChunk = SrcChunks[i];

                    // We only care about chunks we have not visited yet.
                    if (!visitedChunks.TryGetValue(srcChunk.m_Chunk->SequenceNumber, out _))
                    {
                        // This chunk exists in the source world but NOT in the destination world.
                        // This means the chunk was created.
                        CreatedChunks.Add(srcChunk);
                        CreatedChunkFlags.Add(ArchetypeChunkChangeFlags.Cloned);
                        CreatedChunkEntityCounts.Add(createdChunkEntityCounts);
                        createdChunkEntityCounts += srcChunk.m_Chunk->Count;
                    }
                }

                CreatedChunkEntityCounts.Add(createdChunkEntityCounts);
                DestroyedChunkEntityCounts.Add(destroyedChunkEntityCount);
            }

            static bool ChunksAreDifferent(Chunk* srcChunk, Chunk* dstChunk)
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
        /// Constructs a set of changes between the given src and dst chunks.
        /// </summary>
        /// <remarks>
        /// A chunk is considered unchanged if the <see cref="Chunk.SequenceNumber"/> matches and all type change versions match.
        /// </remarks>
        internal static ArchetypeChunkChanges GetArchetypeChunkChanges(
            NativeArray<ArchetypeChunk> srcChunks,
            NativeArray<ArchetypeChunk> dstChunks,
            Allocator allocator,
            out JobHandle jobHandle,
            JobHandle dependsOn = default)
        {
            var archetypeChunkChanges = new ArchetypeChunkChanges(allocator);
            var srcChunksBySequenceNumber = new NativeHashMap<ulong, ArchetypeChunk>(srcChunks.Length, Allocator.TempJob);

            var buildChunkSequenceNumberMap = new BuildChunkSequenceNumberMap
            {
                Chunks = srcChunks,
                ChunksBySequenceNumber = srcChunksBySequenceNumber.AsParallelWriter()
            }.Schedule(srcChunks.Length, 64, dependsOn);

            var gatherArchetypeChunkChanges = new GatherArchetypeChunkChanges
            {
                SrcChunks = srcChunks,
                DstChunks = dstChunks,
                SrcChunksBySequenceNumber = srcChunksBySequenceNumber,
                CreatedChunks = archetypeChunkChanges.CreatedSrcChunks.Chunks,
                CreatedChunkFlags = archetypeChunkChanges.CreatedSrcChunks.Flags,
                CreatedChunkEntityCounts = archetypeChunkChanges.CreatedSrcChunks.EntityCounts,
                DestroyedChunks = archetypeChunkChanges.DestroyedDstChunks.Chunks,
                DestroyedChunkFlags = archetypeChunkChanges.DestroyedDstChunks.Flags,
                DestroyedChunkEntityCounts = archetypeChunkChanges.DestroyedDstChunks.EntityCounts,
            }.Schedule(buildChunkSequenceNumberMap);

            jobHandle = srcChunksBySequenceNumber.Dispose(gatherArchetypeChunkChanges);

            return archetypeChunkChanges;
        }
    }
}
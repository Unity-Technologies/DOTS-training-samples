using System;
using Unity.Collections;

namespace Unity.Entities
{
    internal readonly struct ArchetypeChunkCollection : IDisposable
    {
        private readonly NativeList<ArchetypeChunk> m_Chunks;
        private readonly NativeList<ChunkChangeFlags> m_ChangeFlags;
        private readonly NativeArray<int> m_EntityCountOffsets;
        private readonly int m_EntityCount;

        /// <summary>
        /// A set of chunks within a world.
        /// </summary>
        public NativeArray<ArchetypeChunk> Chunks => m_Chunks.AsArray();
        
        /// <summary>
        /// Change flags for each chunk matched by index.
        /// </summary>
        public NativeArray<ChunkChangeFlags> ChunkChangeFlags => m_ChangeFlags.AsArray();
        
        /// <summary>
        /// An array containing the cumulative count of entities per chunk.
        /// </summary>
        public NativeArray<int> EntityCountOffsets => m_EntityCountOffsets;
        
        /// <summary>
        /// The total sum of <see cref="Chunks"/> entity counts.
        ///
        /// @NOTE This is NOT the total number of entities in the world. 
        /// </summary>
        public int EntityCount => m_EntityCount;

        public ArchetypeChunkCollection(
            NativeList<ArchetypeChunk> chunks,
            NativeList<ChunkChangeFlags> changeFlags,
            NativeArray<int> entityCountOffsets,
            int entityCount)
        {
            m_Chunks = chunks;
            m_ChangeFlags = changeFlags;
            m_EntityCountOffsets = entityCountOffsets;
            m_EntityCount = entityCount;
        }

        public void Dispose()
        {
            m_Chunks.Dispose();
            m_ChangeFlags.Dispose();
            m_EntityCountOffsets.Dispose();
        }
    }
}
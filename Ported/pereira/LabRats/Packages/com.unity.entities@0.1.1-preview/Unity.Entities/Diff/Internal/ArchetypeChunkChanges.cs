using System;

namespace Unity.Entities
{
    /// <summary>
    /// Intermediate structure used to hold a set of created and destroyed chunks.
    /// </summary>
    internal readonly struct ArchetypeChunkChanges : IDisposable
    {
        private readonly ArchetypeChunkCollection m_CreatedSrcChunks;
        private readonly ArchetypeChunkCollection m_DestroyedDstChunks;

        /// <summary>
        /// A set of all chunks in the SrcWorld that have been created.
        ///
        /// This includes modified chunks that are being re-created.
        /// </summary>
        public ArchetypeChunkCollection CreatedSrcChunks => m_CreatedSrcChunks;
        
        /// <summary>
        /// A set of all chunks in the DstWorld that have be destroyed from the SrcWorld.
        ///
        /// This includes modified chunks that are being re-created.
        /// </summary>
        public ArchetypeChunkCollection DestroyedDstChunks => m_DestroyedDstChunks;
        
        /// <summary>
        /// Returns true if any chunk level changes were detected.
        /// </summary>
        public bool HasChanges => m_CreatedSrcChunks.Chunks.Length != 0 || m_DestroyedDstChunks.Chunks.Length != 0;
        
        public ArchetypeChunkChanges(
            ArchetypeChunkCollection srcChunks,
            ArchetypeChunkCollection dstChunks)
        {
            m_CreatedSrcChunks = srcChunks;
            m_DestroyedDstChunks = dstChunks;
        }

        public void Dispose()
        {
            m_CreatedSrcChunks.Dispose();
            m_DestroyedDstChunks.Dispose();
        }
    }
}
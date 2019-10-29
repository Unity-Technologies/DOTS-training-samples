namespace Unity.Entities
{
    internal static class ChunkUtility
    {
        /// <summary>
        /// @HACK
        ///
        /// For some reason in NET_DOTS fixed byte Buffer[4] does not register as pointer.
        /// 
        /// </summary>
        /// <param name="chunk"></param>
        /// <returns></returns>
        public static unsafe byte* GetBuffer(Chunk* chunk)
        {
#if !NET_DOTS
            return chunk->Buffer;
#else
            return (byte*) chunk + Chunk.kBufferOffset;
#endif
        }
    }
}
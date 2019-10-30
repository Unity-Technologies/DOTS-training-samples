using System;
using System.Runtime.InteropServices;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Entities
{
    [Flags]
    internal enum ChunkFlags
    {
        None = 0,
        Locked = 1 << 0,
        Unused = 1 << 1,
        TempAssertWillDestroyAllInLinkedEntityGroup = 1 << 2
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct Chunk
    {
        // Chunk header START
        [FieldOffset(0)]
        public Archetype* Archetype;
        // 4-byte padding on 32-bit architectures here

        [FieldOffset(8)]
        public Entity metaChunkEntity;

        // This is meant as read-only.
        // EntityComponentStore.SetChunkCount should be used to change the count.
        [FieldOffset(16)]
        public int Count;
        [FieldOffset(20)]
        public int Capacity;

        // Archetypes can contain non-ECS-type components which are managed objects.
        // In order to access them without a lot of overhead we conceptually store an Object[] in each chunk which contains the managed components.
        // The chunk does not really own the array though since we cannot store managed references in unmanaged memory,
        // so instead the ManagedComponentStore has a list of Object[]s and the chunk just has an int to reference an Object[] by index in that list.
        [FieldOffset(24)]
        public int ManagedArrayIndex;

        [FieldOffset(28)]
        public int ListIndex;
        [FieldOffset(32)]
        public int ListWithEmptySlotsIndex;
        
        // Special chunk behaviors
        [FieldOffset(36)]
        public uint Flags;

        // Incrementing automatically for each chunk
        [FieldOffset(40)]
        public ulong SequenceNumber;

        // Chunk header END

        // Component data buffer
        // This is where the actual chunk data starts.
        // It's declared like this so we can skip the header part of the chunk and just get to the data.
        public const int kBufferOffset = 64; // (must be cache line aligned)
        [FieldOffset(kBufferOffset)]
        public fixed byte Buffer[4];

        public const int kChunkSize = 16 * 1024 - 256; // allocate a bit less to allow for header overhead
        public const int kBufferSize = kChunkSize - kBufferOffset;
        public const int kMaximumEntitiesPerChunk = kBufferSize / 8;

        public uint GetChangeVersion(int typeIndex)
        {
            return Archetype->Chunks.GetChangeVersion(typeIndex, ListIndex);
        }

        public void SetChangeVersion(int typeIndex, uint version)
        {
            Archetype->Chunks.SetChangeVersion(typeIndex, ListIndex, version);
        }

        public void SetAllChangeVersions(uint version)
        {
            Archetype->Chunks.SetAllChangeVersion(ListIndex, version);
        }

        public int GetSharedComponentValue(int typeOffset)
        {
            return Archetype->Chunks.GetSharedComponentValue(typeOffset, ListIndex);
        }

        public SharedComponentValues SharedComponentValues => Archetype->Chunks.GetSharedComponentValues(ListIndex);

        public static int GetChunkBufferSize()
        {
            // The amount of available space in a chunk is the max chunk size, kChunkSize,
            // minus the size of the Chunk's metadata stored in the fields preceding Chunk.Buffer
            return kChunkSize - kBufferOffset;
        }

        public static Chunk* MallocChunk(Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            Assert.IsTrue(CollectionHelper.IsAligned(kBufferOffset, CollectionHelper.CacheLineSize));
#endif
            return (Chunk*)UnsafeUtility.Malloc(kChunkSize, CollectionHelper.CacheLineSize, allocator);
        }

        public bool MatchesFilter(MatchingArchetype* match, ref EntityQueryFilter filter)
        {
            return match->ChunkMatchesFilter(ListIndex, ref filter);
        }

        public int GetSharedComponentIndex(MatchingArchetype* match, int indexInEntityQuery)
        {
            var componentIndexInArcheType = match->IndexInArchetype[indexInEntityQuery];
            var componentIndexInChunk = componentIndexInArcheType - match->Archetype->FirstSharedComponent;
            return GetSharedComponentValue(componentIndexInChunk);
        }

        /// <summary>
        /// Returns true if Chunk is Locked
        /// </summary>
        public bool Locked => (Flags & (uint) ChunkFlags.Locked) != 0;
    }
}

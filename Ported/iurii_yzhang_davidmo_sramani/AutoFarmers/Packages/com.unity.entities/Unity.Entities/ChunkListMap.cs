using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ChunkListMap : IDisposable
    {
        private const UInt32 kAValidHashCode = 0x00000001;
        private const UInt32 kSkipCode = 0xFFFFFFFF;

        static uint GetHashCode(SharedComponentValues sharedComponentValues, int numSharedComponents)
        {
            UInt32 result;
            if (sharedComponentValues.stride == sizeof(int))
            {
                result = math.hash(sharedComponentValues.firstIndex, numSharedComponents * sizeof(int));
            }
            else
            {
                int* indexArray = stackalloc int[numSharedComponents];

                for (int i = 0; i < numSharedComponents; ++i)
                    indexArray[i] = sharedComponentValues[i];
                result = math.hash(indexArray, numSharedComponents * sizeof(int));
            }

            if (result == 0 || result == kSkipCode)
                result = kAValidHashCode;
            return result;
        }

        private UnsafeUintList hashes;
        private UnsafeChunkPtrList chunks;

        private int emptyNodes;
        private int skipNodes;

        public int Size
        {
            get => hashes.Length;
        }

        public int UnoccupiedNodes
        {
            get => emptyNodes + skipNodes;
        }

        public int OccupiedNodes
        {
            get => Size - UnoccupiedNodes;
        }

        public bool IsEmpty
        {
            get => OccupiedNodes == 0;
        }

        private int hashMask
        {
            get => Size - 1;
        }

        public int MinimumSize
        {
            get => 64 / sizeof(UInt32);
        }

        public void SetCapacity(int capacity)
        {
            if (capacity < MinimumSize)
                capacity = MinimumSize;
            chunks.SetCapacity(capacity);
            hashes.SetCapacity(capacity);
        }

        public void Init(int count)
        {
            if (count < MinimumSize)
                count = MinimumSize;
            Assert.IsTrue(0 == (count & (count - 1)));

            hashes = new UnsafeUintList(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            hashes.Resize(count);

            chunks = new UnsafeChunkPtrList(count, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            chunks.Resize(count);

            emptyNodes = count;
            skipNodes = 0;
        }

        public void AppendFrom(ref ChunkListMap src)
        {
            for (int offset = 0; offset < src.Size; ++offset)
            {
                var hash = src.hashes.Ptr[offset];
                if (hash != 0 && hash != kSkipCode)
                    Add(src.chunks.Ptr[offset]);
            }
        }

        public Chunk* TryGet(SharedComponentValues sharedComponentValues, int numSharedComponents)
        {
            uint desiredHash = GetHashCode(sharedComponentValues, numSharedComponents);
            int offset = (int)(desiredHash & (uint)hashMask);
            int attempts = 0;
            while (true)
            {
                var hash = hashes.Ptr[offset];
                if (hash == 0)
                    return null;
                if (hash == desiredHash)
                {
                    var chunk = chunks.Ptr[offset];
                    if (sharedComponentValues.EqualTo(chunk->SharedComponentValues, numSharedComponents))
                        return chunk;
                }
                offset = (offset + 1) & hashMask;
                ++attempts;
                if (attempts == Size)
                    return null;
            }
        }

        public void Resize(int size)
        {
            if (size < MinimumSize)
                size = MinimumSize;
            if (size == Size)
                return;
            var temp = this;
            this = new ChunkListMap();
            Init(size);
            AppendFrom(ref temp);
            temp.Dispose();
        }

        public void PossiblyGrow()
        {
            if (UnoccupiedNodes < Size / 3)
                Resize(Size * 2);
        }

        public void PossiblyShrink()
        {
            if (OccupiedNodes < Size / 3)
                Resize(Size / 2);
        }

        public void Add(Chunk* chunk)
        {
            Assert.IsTrue(chunk != null);
            Assert.IsTrue(chunk->Archetype != null);
            var sharedComponentValues = chunk->SharedComponentValues;
            int numSharedComponents = chunk->Archetype->NumSharedComponents;
            uint desiredHash = GetHashCode(sharedComponentValues, numSharedComponents);
            int offset = (int)(desiredHash & (uint)hashMask);
            int attempts = 0;
            while (true)
            {
                var hash = hashes.Ptr[offset];
                if (hash == 0)
                {
                    hashes.Ptr[offset] = desiredHash;
                    chunks.Ptr[offset] = chunk;
                    chunk->ListWithEmptySlotsIndex = offset;
                    --emptyNodes;
                    PossiblyGrow();
                    return;
                }

                if (hash == kSkipCode)
                {
                    hashes.Ptr[offset] = desiredHash;
                    chunks.Ptr[offset] = chunk;
                    chunk->ListWithEmptySlotsIndex = offset;
                    --skipNodes;
                    PossiblyGrow();
                    return;
                }

                offset = (offset + 1) & hashMask;
                ++attempts;
                Assert.IsTrue(attempts < Size);
            }
        }

        public void Remove(Chunk* chunk)
        {
            int offset = chunk->ListWithEmptySlotsIndex;
            chunk->ListWithEmptySlotsIndex = -1;
            Assert.IsTrue(offset != -1);
            Assert.IsTrue(chunks.Ptr[offset] == chunk);
            hashes.Ptr[offset] = kSkipCode;
            ++skipNodes;
            PossiblyShrink();
        }

        public bool Contains(Chunk* chunk)
        {
            var offset = chunk->ListWithEmptySlotsIndex;
            return offset != -1 && chunks.Ptr[offset] == chunk;
        }

        public void Dispose()
        {
            hashes.Dispose();
            chunks.Dispose();
            emptyNodes = 0;
            skipNodes = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ArchetypeListMap : IDisposable
    {
        private const UInt32 kAValidHashCode = 0x00000001;
        private const UInt32 kSkipCode = 0xFFFFFFFF;

        static uint GetHashCode(ComponentTypeInArchetype* type, int types)
        {
            UInt32 result = math.hash(type, types * sizeof(ComponentTypeInArchetype));
            if (result == 0 || result == kSkipCode)
                result = kAValidHashCode;
            return result;
        }

        public UnsafeUintList hashes;
        public UnsafeArchetypePtrList archetypes;
        public int emptyNodes;
        public int skipNodes;

        public int Size
        {
            get => hashes.Length;
        }

        public int UnoccupiedNodes
        {
            get => emptyNodes + skipNodes;
        }

        public int OccupiedNodes
        {
            get => Size - UnoccupiedNodes;
        }

        public bool IsEmpty
        {
            get => OccupiedNodes == 0;
        }

        private int hashMask
        {
            get => Size - 1;
        }

        public int MinimumSize
        {
            get => 64 / sizeof(UInt32);
        }

        public void SetCapacity(int capacity)
        {
            if (capacity < MinimumSize)
                capacity = MinimumSize;
            archetypes.SetCapacity(capacity);
            hashes.SetCapacity(capacity);
        }

        public void Init(int count)
        {
            if (count < MinimumSize)
                count = MinimumSize;
            Assert.IsTrue(0 == (count & (count - 1)));

            hashes = new UnsafeUintList(count, Allocator.Persistent);
            hashes.Resize(count, NativeArrayOptions.ClearMemory);

            archetypes = new UnsafeArchetypePtrList(count, Allocator.Persistent);
            archetypes.Resize(count);

            emptyNodes = count;
            skipNodes = 0;
        }

        public void AppendFrom(ArchetypeListMap* src)
        {
            for (int offset = 0; offset < src->Size; ++offset)
            {
                var hash = src->hashes.Ptr[offset];
                if (hash != 0 && hash != kSkipCode)
                    Add(src->archetypes.Ptr[offset]);
            }
            src->Dispose();
        }

        public Archetype* TryGet(ComponentTypeInArchetype* type, int types)
        {
            uint desiredHash = GetHashCode(type, types);
            int offset = (int)(desiredHash & (uint)hashMask);
            int attempts = 0;
            while (true)
            {
                var hash = hashes.Ptr[offset];
                if (hash == 0)
                    return null;
                if (hash == desiredHash)
                {
                    var archetype = archetypes.Ptr[offset];
                    if (archetype->TypesCount == types && 0 == UnsafeUtility.MemCmp(archetype->Types, type, types * sizeof(ComponentTypeInArchetype)))
                        return archetype;
                }
                offset = (offset + 1) & hashMask;
                ++attempts;
                if (attempts == Size)
                    return null;
            }
        }

        public Archetype* Get(ComponentTypeInArchetype* type, int types)
        {
            var result = TryGet(type, types);
            Assert.IsFalse(result == null);
            return result;
        }

        public void Resize(int size)
        {
            if (size < MinimumSize)
                size = MinimumSize;
            if (size == Size)
                return;
            var temp = this;
            this = new ArchetypeListMap();
            Init(size);
            AppendFrom(&temp);
            temp.Dispose();
        }

        public void PossiblyGrow()
        {
            if (UnoccupiedNodes < Size / 3)
                Resize(Size * 2);
        }

        public void PossiblyShrink()
        {
            if (OccupiedNodes < Size / 3)
                Resize(Size / 2);
        }

        public void Add(Archetype* archetype)
        {
            uint desiredHash = GetHashCode(archetype->Types, archetype->TypesCount);
            int offset = (int)(desiredHash & (uint)hashMask);
            int attempts = 0;
            while (true)
            {
                var hash = hashes.Ptr[offset];
                if (hash == 0)
                {
                    hashes.Ptr[offset] = desiredHash;
                    archetypes.Ptr[offset] = archetype;
                    --emptyNodes;
                    PossiblyGrow();
                    return;
                }

                if (hash == kSkipCode)
                {
                    hashes.Ptr[offset] = desiredHash;
                    archetypes.Ptr[offset] = archetype;
                    --skipNodes;
                    PossiblyGrow();
                    return;
                }

                offset = (offset + 1) & hashMask;
                ++attempts;
                Assert.IsTrue(attempts < Size);
            }
        }

        public int IndexOf(Archetype* archetype)
        {
            uint desiredHash = GetHashCode(archetype->Types, archetype->TypesCount);
            int offset = (int)(desiredHash & (uint)hashMask);
            uint attempts = 0;
            while (true)
            {
                var hash = hashes.Ptr[offset];
                if (hash == 0)
                    return -1;
                if (hash == desiredHash)
                {
                    var c = archetypes.Ptr[offset];
                    if (c == archetype)
                        return offset;
                }
                offset = (offset + 1) & hashMask;
                ++attempts;
                if (attempts == Size)
                    return -1;
            }
        }

        public void Remove(Archetype* archetype)
        {
            int offset = IndexOf(archetype);
            Assert.IsTrue(offset != -1);
            hashes.Ptr[offset] = kSkipCode;
            ++skipNodes;
            PossiblyShrink();
        }

        public bool Contains(Archetype* archetype)
        {
            return IndexOf(archetype) != -1;
        }

        public void Dispose()
        {
            hashes.Dispose();
            archetypes.Dispose();
            emptyNodes = 0;
            skipNodes = 0;
        }
    }
}

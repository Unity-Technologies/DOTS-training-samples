using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Mathematics;

namespace Unity.Entities.Determinism
{
    internal static class ArchetypeChunkExtensions
    {
        public static NativeView GetComponentView(this ArchetypeChunk self, ComponentType type)
        {
            if (type.IsZeroSized)
            {
                throw new ArgumentException("ArchetypeChunk.GetComponentView can not be called with zero sized types");
            }

            if (type.IsBuffer)
            {
                throw new ArgumentException("ArchetypeChunk.GetComponentView can not be called with buffer types");
            }

            return GetComponentView(self, self.GetTypeIndexInChunk(type));
        }

        internal static NativeView GetComponentView(this ArchetypeChunk self, ChunkTypeIndex index)
        {
            var count = self.Count;
            var typeSize = self.GetTypeSize(index);
            var lengthInBytes = typeSize * count;
            
            var offset = self.GetTypeOffset(index);
                
            unsafe
            {
                var ptr = self.m_Chunk->Buffer + offset;
                return new NativeView(ptr, lengthInBytes);
            }            
        }

        public static NativeArray<NativeView> GetBufferViews(this ArchetypeChunk self, ComponentType type, Allocator allocator)
        {
            if (!type.IsBuffer)
            {
                throw new ArgumentException("ArchetypeChunk.GetBufferView can only be called with buffer types");
            }
            
            var elementSize = TypeManager.GetTypeInfo(type.TypeIndex).ElementSize;
            var chunkIndex = self.GetTypeIndexInChunk(type);
            
            return GetBufferViews(self, chunkIndex, elementSize,allocator);
        }

        internal static NativeArray<NativeView> GetBufferViews(this ArchetypeChunk self, ChunkTypeIndex index, int elementSize, Allocator allocator)
        {
            var typeSize = self.GetTypeSize(index);
            var offset = self.GetTypeOffset(index);
        
            var count = self.Count;
            var views = new NativeArray<NativeView>(count, allocator, NativeArrayOptions.UninitializedMemory);

            unsafe
            {
                var ptr = self.m_Chunk->Buffer + offset;
                
                for (int i = 0; i < count; i++)
                {
                    var buffer = (BufferHeader*) (ptr + i * typeSize);
                    var data = BufferHeader.GetElementPointer(buffer);
                    
                    views[i] = new NativeView(data, buffer->Length * elementSize);
                }
            }
            return views;
        }

        internal static int GetTypeSize(this ArchetypeChunk self, ChunkTypeIndex index)
        {
            unsafe
            {
                return self.m_Chunk->Archetype->SizeOfs[index.Value];
            }
        }

        internal static int GetTypeOffset(this ArchetypeChunk self, ChunkTypeIndex index)
        {
            unsafe
            {
                return self.m_Chunk->Archetype->Offsets[index.Value];
            }
        }

        internal static ChunkTypeIndex GetTypeIndexInChunk(this ArchetypeChunk self, ComponentType type)
        {
            unsafe
            {
                return new ChunkTypeIndex
                {
                    Value = ChunkDataUtility.GetIndexInTypeArray(self.m_Chunk->Archetype, type.TypeIndex)
                };
            }
        }
        
        internal static NativeArray<ComponentType> GetNonzeroComponentTypes(this ArchetypeChunk self, Allocator allocator)
        {
            var nonZeroTypeCount = self.GetNonZeroSizedTypeCount();
            var result = new NativeArray<ComponentType>(nonZeroTypeCount, allocator);

            for (int i = 0; i < nonZeroTypeCount; i++)
            {
                // nonzero types are stored at the beginning followed by all zero-sized types
                result[i] = self.GetComponentTypeAtIndex(i);
            }
            
            return result;
        }

        public static int GetNonZeroSizedTypeCount(this ArchetypeChunk self)
        {
            unsafe
            {
                return self.m_Chunk->Archetype->NonZeroSizedTypesCount;
            }
        }

        public static int GetTypeCount(this ArchetypeChunk self)
        {
            unsafe
            {
                return self.m_Chunk->Archetype->TypesCount;
            }
        }
        
        internal static uint GetChangeVersion(this ArchetypeChunk self, int index)
        {
            unsafe
            {
                return self.m_Chunk->GetChangeVersion(index);
            }
        }

        public static ulong GetSequenceNumber(this ArchetypeChunk self)
        {
            unsafe
            {
                return self.m_Chunk->SequenceNumber;
            }
        }

        internal static ComponentType GetComponentTypeAtIndex(this ArchetypeChunk self, int typeIndex)
        {
            unsafe
            {
                return self.m_Chunk->Archetype->Types[typeIndex].ToComponentType();
            }
        }

        [Serializable]
        internal struct GatheredChunkHeader
        {
            public uint ComponentVersionHash;
            public int ChunkHashFlags;
            
            public int EntityCountInChunk;
            public int SharedComponentCount;
            
            public int TotalTypeCount;
            public uint Seed;
        }

        internal static GatheredChunkHeader GetGatheredChunkHeader(this ArchetypeChunk self, uint seed)
        {
            return new GatheredChunkHeader
            {
                ComponentVersionHash = self.GetComponentVersionHash(seed),
                ChunkHashFlags = self.Locked() ? 1 : 0,
                EntityCountInChunk = self.Count,
                SharedComponentCount = self.NumSharedComponents(),
                TotalTypeCount = self.GetTypeCount(),
                Seed = seed,
            };
        }

        internal static uint GetComponentVersionHash(this ArchetypeChunk self, uint seed)
        {
            var hash = seed;
            var typeCount = self.GetTypeCount();
            for (int i = 0; i < typeCount; i++)
            {
                var changeVersion = self.GetChangeVersion(i);
                hash = math.hash(new uint2(changeVersion, hash));
            }
            return hash;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckValidTypeIndexOrThrow(ChunkTypeIndex index, int typeCount)
        {
            // todo: improve error message (which would currently not be burstable), e.g.:
            // throw new ArgumentException($"Type {TypeManager.GetType(type.TypeIndex)} is not part of this Archetype");
            
            if (index.Value < 0)
            {
                throw new ArgumentException("Type is not part of this Archetype");
            }

            if (index.Value >= typeCount)
            {
                throw new ArgumentException("Index exceeds type count");
            }
        }
    }
    
    internal struct ChunkTypeIndex
    {
        public int Value;
    }
}

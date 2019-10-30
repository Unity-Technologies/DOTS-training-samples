using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities.Serialization;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Entities.Determinism
{
            
    internal interface IHasher
    {
        uint Execute(NativeView view, ComponentType type, uint seed);
    }

    [BurstCompile]
    internal struct MaskedHasher : IHasher
    {
        [ReadOnly] public PaddingMasks Masks;
            
        public uint Execute(NativeView view, ComponentType type, uint seed)
        {
            return HashUtility.GetMaskedHash(view, Masks.GetMaskView(type), seed);
        }
    }
       
    [BurstCompile]
    internal struct DenseHasher : IHasher
    {
        public uint Execute(NativeView view, ComponentType _, uint seed)
        {
            return HashUtility.GetDenseHash(view, seed);
        }
    }
    
    [BurstCompile]
    internal static class HashUtility
    {
        public static uint GetWorldHash<T>(World world, T hasher, uint seed)
        where T : struct, IHasher
        {
            if (!world.IsCreated)
            {
                return seed;
            }

            world.EntityManager.BeforeStructuralChange();
            
            var hashes = GetWorldHashes_IJobChunk(world, hasher, seed, Allocator.TempJob);
            
            var gatheredHash = GetDenseHash(hashes, seed);
            hashes.Dispose();

            return gatheredHash;
        }
        
        public static NativeArray<uint> GetChunkHashes<T>(ArchetypeChunk chunk, T hasher, uint seed) where T : struct, IHasher
        {
            var types = chunk.GetNonzeroComponentTypes(Allocator.Temp);

            var typeCount = types.Length;
            var hashes = new NativeArray<uint>(typeCount + 1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < typeCount; i++)
            {
                var chunkIndex = new ChunkTypeIndex
                {
                    Value = i
                };

                var type = types[i];

                if (type.IsBuffer)
                {
                    var elementSize = TypeManager.GetTypeInfo(type.TypeIndex).ElementSize;
                    var bufferViews = chunk.GetBufferViews(chunkIndex, elementSize, Allocator.Temp);

                    var bufferCount = bufferViews.Length;
                    var bufferHashes = new NativeArray<uint>(bufferCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

                    for (int j = 0; j < bufferCount; j++)
                    {
                        bufferHashes[j] = hasher.Execute(bufferViews[j], type, seed);
                    }

                    hashes[i] = GetDenseHash(bufferHashes, seed);

                    bufferHashes.Dispose();
                    bufferViews.Dispose();
                }
                else
                {
                    var view = chunk.GetComponentView(chunkIndex);
                    hashes[i] = hasher.Execute(view, type, seed);
                }
            }

            hashes[typeCount] = GetChunkHeaderHash(chunk, seed);
            types.Dispose();

            return hashes;
        }

        public static uint GetChunkHash<T>(ArchetypeChunk chunk, T hasher,uint seed) where T : struct, IHasher
        {
            var chunkHashes = GetChunkHashes(chunk, hasher, seed);
            var hash = GetDenseHash(chunkHashes, seed);
            chunkHashes.Dispose();

            return hash;
        }

        public static uint GetMaskedHash<T>(NativeArray<T> data, PaddingMasks masks, uint seed) where T : struct
        {
            return GetMaskedHash(data, masks.GetTypeMask<T>(), seed);
        }

        public static uint GetMaskedHash(NativeArray<byte> data, TypeManager.TypeInfo type, PaddingMasks masks, uint seed)
        {
            return GetMaskedHash(data, masks.GetTypeMask(type), seed);
        }

        public static uint GetMaskedHash(NativeArray<byte> data, ComponentType type, PaddingMasks masks, uint seed)
        {
            return GetMaskedHash(data, masks.GetTypeMask(type), seed);
        }

        static uint GetMaskedHash<T>(NativeArray<T> data, NativeArray<byte> mask, uint seed) where T : struct
        {
            return GetMaskedHash(NativeViewUtility.GetReadView(data), NativeViewUtility.GetReadView(mask), seed);
        }

        internal static uint GetMaskedHash(NativeView data, NativeView mask, uint seed)
        {
            var copy = NativeArrayUtility.CreateOwningCopy(data, Allocator.Temp);
            MaskedMemCopy.ApplyMask(NativeViewUtility.GetWriteView(copy), mask);
            var hash = GetDenseHash(copy, seed);

            copy.Dispose();
            return hash;
        }

        public static uint GetDenseHash<T>(NativeArray<T> data, uint seed) where T : struct
        {
            return GetDenseHash(NativeViewUtility.GetReadView(data), seed);
        }

        internal static uint GetDenseHash(NativeView buffer, uint seed)
        {
            if (buffer.IsEmpty) return seed;

            unsafe
            {
                return math.hash(buffer.Ptr, buffer.LengthInBytes, seed);
            }
        }

        internal static uint GetChunkHeaderHash(ArchetypeChunk chunk, uint seed)
        {
            var header = chunk.GetGatheredChunkHeader(seed);
            return GetDenseHash(NativeViewUtility.GetView(ref header), seed);
        }

        internal static NativeArray<uint> GetWorldHashes_ParallelFor<T>(World world, T hasher, uint seed, Allocator allocator)
        where T : struct, IHasher 
        {
            IsValidJobAllocatorOrThrow(allocator);

            using (var chunks = world.EntityManager.GetAllChunks())
            {
                var chunkCount = chunks.Length;
                var hashes = new NativeArray<uint>(chunkCount, allocator, NativeArrayOptions.UninitializedMemory);

                new HashChunks_ParallelFor<T>
                    {
                        Chunks = chunks,
                        Hashes = hashes,
                        Seed = seed,
                        Hasher = hasher
                    }
                    .Schedule(chunkCount, 1)
                    .Complete();

                return hashes;
            }
        }

        internal static NativeArray<uint> GetWorldHashes_IJobChunk<T>(World world, T hasher, uint seed, Allocator allocator)
        where T : struct, IHasher
        {
            IsValidJobAllocatorOrThrow(allocator);

            var query = world.EntityManager.UniversalQuery;
            var chunkCount = query.CalculateChunkCountWithoutFiltering();

            var hashes = new NativeArray<uint>(chunkCount, allocator, NativeArrayOptions.UninitializedMemory);

            new HashChunks_IJobChunk<T>
                {
                    Hashes = hashes,
                    Hasher = hasher,
                    Seed = seed
                }
                .Schedule(query)
                .Complete();

            return hashes;
        }
        
        [BurstCompile]
        struct HashChunks_IJobChunk<T> : IJobChunk
        where T : struct, IHasher
        {
            [WriteOnly] public NativeArray<uint> Hashes;

            [ReadOnly] public uint Seed;
            
            [ReadOnly] public T Hasher;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int _)
            {
                Hashes[chunkIndex] = GetChunkHash(chunk, Hasher, Seed);
            }
        }


        [BurstCompile]
        struct HashChunks_ParallelFor<T> : IJobParallelFor
        where T : struct, IHasher
        {
            [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
            [WriteOnly] public NativeArray<uint> Hashes;

            [ReadOnly] public uint Seed;
            
            [ReadOnly] public T Hasher;

            public void Execute(int i) => Hashes[i] = GetChunkHash(Chunks[i], Hasher, Seed);
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void IsValidJobAllocatorOrThrow(Allocator allocator)
        {
            if (allocator < Allocator.TempJob)
            {
                throw new ArgumentException(
                    "Allocated memory will be used in job, please use TempJob or Persistent allocators");
            }
        }
    }

    internal static class InternallyExposedPerformanceTestExtensions
    {
        public static NativeArray<byte> GetContentAsNativeArray(MemoryBinaryWriter writer) => writer.GetContentAsNativeArray();
    }
}

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;

namespace Unity.Entities.Determinism
{
    internal static class LocalToWorldHash
    {
        public static uint HashTransforms(World world, uint seed)
        {
            if (!world.IsCreated) return seed;

            var entities = world.EntityManager;
            using (var query = entities.CreateEntityQuery(ComponentType.ReadOnly<LocalToWorld>()))
            using( var chunks = query.CreateArchetypeChunkArray(Allocator.TempJob) )
            using( var hashes = new NativeArray<uint>(chunks.Length, Allocator.TempJob) )
            {
                if (0 == chunks.Length) return seed;

                new HashLocalToWorldChunksJob
                    {
                        Access = entities.GetArchetypeChunkComponentType<LocalToWorld>(true),
                        Chunks = chunks,
                        Hashes = hashes,
                        Seed = seed
                    }
                    .Schedule(chunks.Length, 1)
                    .Complete();

                return HashUtility.GetDenseHash(hashes, seed);
            }
        }

        [BurstCompile]
        struct HashLocalToWorldChunksJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<ArchetypeChunk> Chunks;

            [ReadOnly]
            public ArchetypeChunkComponentType<LocalToWorld> Access;

            [WriteOnly]
            public NativeArray<uint> Hashes;

            public uint Seed;

            public void Execute(int index)
            {
                var data = Chunks[index].GetNativeArray(Access);
                Hashes[index] = HashUtility.GetDenseHash(data, Seed);
            }
        }
    }

}

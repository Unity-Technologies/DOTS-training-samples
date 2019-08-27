using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Collections;

public class SpawnSystem : JobComponentSystem
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    private EntityQuery m_Query;

    private Random m_Random = new Random(1);

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(LbSpawner), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Rotation>());
        m_CommandBufferSystem = World.GetOrCreateSystem<LbSimulationBarrier>();
    }

    struct SpawnJobChunk : IJobChunk
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;
        public float DeltaTime;
        public int Seed;

        public ArchetypeChunkComponentType<LbSpawner> SpawnerType;
        [ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
        [ReadOnly] public ArchetypeChunkComponentType<Rotation> RotationType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            uint newSeed = (uint)(Seed + chunkIndex * 1000);
            Random chunkRandom = new Random(newSeed);

            var spawners = chunk.GetNativeArray(SpawnerType);
            var translations = chunk.GetNativeArray(TranslationType);
            var rotations = chunk.GetNativeArray(RotationType);

            for (var i=0; i<chunk.Count; ++i)
            {
                var spawner = spawners[i];

                var location = translations[i].Value;
                var rotation = rotations[i].Value;

                spawner.ElapsedTimeForMice += DeltaTime;
                spawner.ElapsedTimeForCats += DeltaTime;

                if (spawner.ElapsedTimeForMice > spawner.MouseFrequency)
                {
                    spawner.ElapsedTimeForMice = 0;

                    var randomDirection = chunkRandom.NextInt(0, 4);
                    DoSpawn(chunkIndex, location, rotation, ref spawner.MousePrefab, randomDirection, 4.0f, false);
                }

                if (spawner.ElapsedTimeForCats > spawner.CatFrequency)
                {
                    spawner.ElapsedTimeForCats = 0;

                    var randomDirection = chunkRandom.NextInt(0, 4);
                    DoSpawn(chunkIndex, location, rotation, ref spawner.CatPrefab, randomDirection, 1.0f, true);
                }

                spawners[i] = spawner;
            }
        }

        private void DoSpawn(int index, float3 translation, quaternion rotation, ref Entity entityType, int direction, float speed, bool isCat)
        {
            var instance = CommandBuffer.Instantiate(index, entityType);

            CommandBuffer.SetComponent(index, instance, new Translation { Value = translation });
            CommandBuffer.SetComponent(index, instance, new Rotation { Value = rotation });

            CommandBuffer.AddComponent<LbRotationSpeed>(index, instance);
            CommandBuffer.AddComponent(index, instance, new LbMovementSpeed { Value = speed });
            CommandBuffer.AddComponent(index, instance, new LbMovementTarget() { From = translation, To = translation });
            CommandBuffer.AddComponent(index, instance, new LbDistanceToTarget { Value = 1.0f });

            CommandBuffer.AddComponent(index, instance, new LbDirection() { Value = (byte)direction });

            if (isCat)
            {
                CommandBuffer.AddComponent<LbCat>(index, instance);
                CommandBuffer.AddComponent(index, instance, new LbLifetime() { Value = 30.0f });
            }
            else
            {
                CommandBuffer.AddComponent<LbRat>(index, instance);
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var jobHandle = new SpawnJobChunk
        {
            DeltaTime = Time.deltaTime,
            CommandBuffer = m_CommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            Seed = m_Random.NextInt(),

            SpawnerType = GetArchetypeChunkComponentType<LbSpawner>(),
            TranslationType = GetArchetypeChunkComponentType<Translation>(),
            RotationType = GetArchetypeChunkComponentType<Rotation>(),

        }.Schedule(m_Query, inputDeps);

        m_CommandBufferSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}

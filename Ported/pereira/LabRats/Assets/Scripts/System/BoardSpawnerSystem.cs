using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Collections;
using Unity.Burst;

public class BoardSpawnerSystem : JobComponentSystem
{
    private EntityQuery m_Query;
    private EntityQuery m_GeneratorQuery;
    private EntityQuery m_BoardQuery;

    private EntityCommandBufferSystem m_Barrier;

    private Random m_Random;

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(LbGameSpawnAll));
        m_GeneratorQuery = GetEntityQuery(typeof(LbBoardGenerator));
        m_BoardQuery = GetEntityQuery(ComponentType.ReadOnly<LbBoard>(), ComponentType.ReadOnly<LbDirectionMap>());

        m_Barrier = World.GetOrCreateSystem<LbCreationBarrier>();

        m_Random = new Random();
        m_Random.InitState(2000);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (m_Query.CalculateEntityCount() <= 0)
            return inputDeps;

        var generator = m_GeneratorQuery.GetSingleton<LbBoardGenerator>();

        var board = m_BoardQuery.GetSingleton<LbBoard>();
        var boardEntity = m_BoardQuery.GetSingletonEntity();
        var bufferLookup = GetBufferFromEntity<LbDirectionMap>();

        var buffer = bufferLookup[boardEntity];
        var bufferArray = buffer.AsNativeArray();


        var spawnerHandle = new BoardSpawnerJob()
        {
            Generator = generator,
            Seed = m_Random.NextUInt(),
            DirectionBuffer = bufferLookup[boardEntity].AsNativeArray(),
            CommandBuffer = m_Barrier.CreateCommandBuffer()
        }.Schedule(inputDeps);

        var cleanUpHandle = new BoardSpawnerCleanJob()
        {
            CommandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent(),
        }.Schedule(this, spawnerHandle);

        m_Barrier.AddJobHandleForProducer(cleanUpHandle);

        return cleanUpHandle;
    }

    struct BoardSpawnerCleanJob : IJobForEachWithEntity<LbGameSpawnAll>
    {
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public void Execute(Entity entity, int index,[ReadOnly] ref LbGameSpawnAll c0)
        {
            CommandBuffer.DestroyEntity(index, entity);
        }
    }

    struct BoardSpawnerJob : IJob
    {
        private const short kHoleFlag = 0x100;
        private const short kHomebaseFlag = 0x800;

        [ReadOnly] public LbBoardGenerator Generator;
        [ReadOnly] public uint Seed;

        [ReadOnly] public NativeArray<LbDirectionMap> DirectionBuffer;

        public EntityCommandBuffer CommandBuffer;

        public void Execute()
        {
            PlaceSpawner(ref CommandBuffer, Generator.Player1Cursor, int2.zero);
            PlaceSpawner(ref CommandBuffer, Generator.Player2Cursor, int2.zero);
            PlaceSpawner(ref CommandBuffer, Generator.Player3Cursor, int2.zero);
            PlaceSpawner(ref CommandBuffer, Generator.Player4Cursor, int2.zero);

            var spawnLocation1 = new int2(0, 0);
            var spawnLocation2 = new int2(Generator.SizeX - 1, 0);
            var spawnLocation3 = new int2(0, Generator.SizeY - 1);
            var spawnLocation4 = new int2(Generator.SizeX - 1, Generator.SizeY - 1);

            PlaceSpawner(ref CommandBuffer, Generator.SpawnerPrefab, spawnLocation1);
            PlaceSpawner(ref CommandBuffer, Generator.SpawnerPrefab, spawnLocation2);
            PlaceSpawner(ref CommandBuffer, Generator.SpawnerPrefab, spawnLocation3);
            PlaceSpawner(ref CommandBuffer, Generator.SpawnerPrefab, spawnLocation4);

            var random = new Random();
            random.InitState(Seed);

            var checkFlag = kHoleFlag | kHomebaseFlag;

            var spawnerMap = new NativeHashMap<int2, byte>(Generator.AdditionalSpawners + 4, Allocator.Temp);
            spawnerMap.TryAdd(spawnLocation1, 1);
            spawnerMap.TryAdd(spawnLocation2, 1);
            spawnerMap.TryAdd(spawnLocation3, 1);
            spawnerMap.TryAdd(spawnLocation4, 1);

            var remaining = Generator.AdditionalSpawners;
            while (remaining > 0)
            {
                var coord = new int2(random.NextInt(Generator.SizeX), random.NextInt(Generator.SizeY));
                var index = coord.y * Generator.SizeY + coord.x;

                // Avoid placing spawners in holes, homebases and on top of other spawners
                var cellMapValue = DirectionBuffer[index].Value & checkFlag;
                if (cellMapValue == kHoleFlag || cellMapValue == kHomebaseFlag || spawnerMap.ContainsKey(coord))
                {
                    continue;
                }

                PlaceSpawner(ref CommandBuffer, Generator.SpawnerPrefab, coord);
                spawnerMap.TryAdd(coord, 1);
                remaining--;
            }

            spawnerMap.Dispose();

            var entity = CommandBuffer.CreateEntity();
            CommandBuffer.AddComponent(entity, new LbGameTimer() { Value = LbConstants.GameTime });
        }
    }

    #region HELPERS
    public static void PlaceSpawner(ref EntityCommandBuffer buffer, Entity prefab, int2 coord)
    {
        var center = new float3(
            coord.x,
            0.75f,                   // Change when we have a height variable
            coord.y);

        var entity = buffer.Instantiate(prefab);
        buffer.SetComponent(entity, new Translation() { Value = center });
    }
    #endregion
}

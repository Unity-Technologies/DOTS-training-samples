using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Random = Unity.Mathematics.Random;

public class PlayerCursorSystem : JobComponentSystem
{
    private EntityCommandBufferSystem m_Barrier;

    private EntityQuery m_Query;
    private EntityQuery m_BoardQuery;

    private Random m_Random = new Random(10);

    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(ComponentType.ReadOnly<LbArrowPrefab>(), typeof(LbMovementTarget), typeof(LbDistanceToTarget));
        m_BoardQuery = GetEntityQuery(ComponentType.ReadOnly<LbBoard>());

        m_Barrier = World.GetOrCreateSystem<LbSimulationBarrier>();
    }

    struct PlayerCursorJob : IJobChunk
    {
        [ReadOnly] public int2 BoardSize;
        [ReadOnly] public int Seed;
        public EntityCommandBuffer.Concurrent CommandBuffer;

        public ArchetypeChunkComponentType<LbMovementTarget> MovementType;
        public ArchetypeChunkComponentType<LbDistanceToTarget> DistanceType;
        [ReadOnly] public ArchetypeChunkComponentType<LbArrowPrefab> ArrowPrefabType;
        [ReadOnly] public ArchetypeChunkComponentType<LbPlayer> PlayerType;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var random = new Unity.Mathematics.Random();
            random.InitState((uint)(Seed + chunkIndex * 10000));

            var movements = chunk.GetNativeArray(MovementType);
            var distances = chunk.GetNativeArray(DistanceType);
            var prefabs = chunk.GetNativeArray(ArrowPrefabType);
            var players = chunk.GetNativeArray(PlayerType);

            for (int i=0; i<chunk.Count; ++i)
            {
                var distance = distances[i];
                if (distance.Value < 1.0f)
                    continue;

                distance.Value = 0.0f;
                distances[i] = distance;

                var movement = movements[i];
                var newPosition = new float3(random.NextInt(0, BoardSize.x), 1.0f, random.NextInt(0, BoardSize.y));
                movement.From = movement.To;
                movement.To = newPosition;
                movements[i] = movement;

                var entity = CommandBuffer.CreateEntity(chunkIndex);
                CommandBuffer.AddComponent(chunkIndex, entity, new LbArrowSpawner()
                {
                    Prefab = prefabs[i].Value,
                    PlayerId = players[i].Value,
                    Direction = (byte)random.NextInt(0, 4),
                    Location = movement.From
                });
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var boardCount = m_BoardQuery.CalculateEntityCount();
        if (boardCount <= 0)
            return inputDeps;

        var board = m_BoardQuery.GetSingleton<LbBoard>();
        float deltaTime = Mathf.Clamp(Time.deltaTime, 0.0f, 0.3f);

        var handle = new PlayerCursorJob()
        {
            BoardSize = new int2(board.SizeX, board.SizeY),
            Seed = m_Random.NextInt(),
            CommandBuffer = m_Barrier.CreateCommandBuffer().ToConcurrent(),

            MovementType = GetArchetypeChunkComponentType<LbMovementTarget>(),
            DistanceType = GetArchetypeChunkComponentType<LbDistanceToTarget>(),
            ArrowPrefabType = GetArchetypeChunkComponentType<LbArrowPrefab>(),
            PlayerType = GetArchetypeChunkComponentType<LbPlayer>(),
        }.Schedule(m_Query, inputDeps);

        return handle;
    }
}
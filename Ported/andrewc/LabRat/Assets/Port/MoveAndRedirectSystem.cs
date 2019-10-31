using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

class MoveAndRedirectSystem : JobComponentSystem
{
    EntityQuery m_Query;

    struct MoveJob : IJobChunk
    {
        public ArchetypeChunkComponentType<Translation> TranslationType;
        public ArchetypeChunkComponentType<Direction> DirectionType;
        [ReadOnly] public ArchetypeChunkComponentType<Speed> SpeedType;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public Board Board;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var chunkTranslations = chunk.GetNativeArray(TranslationType);
            var chunkDirections = chunk.GetNativeArray(DirectionType);
            var chunkSpeeds = chunk.GetNativeArray(SpeedType);

            for (var i = 0; i < chunk.Count; i++)
            {
                var translation = chunkTranslations[i].Value;
                var direction = chunkDirections[i].Value;
                var speed = chunkSpeeds[i].Value;

                int dim = direction.IsVertical() ? 2 : 0;
                int2 tileIndex = Board.ConvertWorldToTileCoordinates(translation);
                float3 translationDelta = DeltaTime * speed * direction.GetWorldDirection();
                float3 tileCenter = new float3((float)tileIndex.x, 0.0f, (float)tileIndex.y);
                float3 translationEnd = translation + translationDelta;
                float timeToHitCenter = (translation[dim] - tileCenter[dim]) / translationDelta[dim];

                if (math.trunc(translationEnd[dim]) != math.trunc(translation[dim]) || (translation[dim] < 0.0f) != (translationEnd[dim] < 0.0f))//timeToHitCenter > 0.0f && timeToHitCenter <= 1.0f)
                {
                    var tile = Board[tileIndex.x, tileIndex.y];

                    eDirection newDirection = direction;
                    if (tile.TileType == eTileType.DirectionArrow && !tile.HasWall(tile.Direction))
                    {
                        newDirection = tile.Direction;
                    }
                    else if (tile.HasWall(direction))
                    {
                        newDirection = direction.RotateClockwise();
                        if (tile.HasWall(newDirection))
                        {
                            newDirection = direction.RotateCounterClockwise();
                            if (tile.HasWall(newDirection))
                                newDirection = direction.Flip();
                        }
                    }

                    if (newDirection != direction)
                    {
                        chunkTranslations[i] = new Translation
                        {
                            Value = tileCenter + (1.0f - timeToHitCenter) * speed * DeltaTime * newDirection.GetWorldDirection()
                        };
                        chunkDirections[i] = new Direction
                        {
                            Value = newDirection
                        };
                    }
                }

                chunkTranslations[i] = new Translation
                {
                    Value = translation + DeltaTime * speed * direction.GetWorldDirection()
                };
            }
        }
    }

    protected override void OnCreate()
    {
        m_Query = EntityManager.CreateEntityQuery(ComponentType.ReadWrite<Direction>(), ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<Speed>());
    }

    protected override JobHandle OnUpdate(JobHandle deps)
    {
        var job = new MoveJob()
        {
            TranslationType = GetArchetypeChunkComponentType<Translation>(false),
            DirectionType = GetArchetypeChunkComponentType<Direction>(false),
            SpeedType = GetArchetypeChunkComponentType<Speed>(false),
            DeltaTime = Time.deltaTime,
            Board = World.GetExistingSystem<BoardSystem>().Board
        };
        return job.Schedule(m_Query, deps);
    }
}

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

                float translationStart = direction.IsVertical() ? translation.z : translation.x;
                float translationDelta = DeltaTime * speed;
                if ((int)translationStart != (int)(translationStart + translationDelta))
                {
                    float3 pos = translation;
                    float remainingDistance = 0.0f;
                    if (direction.IsVertical())
                    {
                        pos.z = math.round(translationStart + translationDelta);
                        remainingDistance = translationStart + translationDelta - pos.z;
                    }
                    else
                    {
                        pos.x = math.round(translationStart + translationDelta);
                        remainingDistance = translationStart + translationDelta - pos.x;
                    }

                    int2 tileIndex = Board.ConvertWorldToTileCoordinates(pos);
                    var tile = Board[tileIndex.x, tileIndex.y];

                    eDirection newDirection = direction;
                    if (tile.TileType == eTileType.DirectionArrow && !tile.HasWall(tile.Direction))
                    {
                        newDirection = tile.Direction;
                    }
                    else if (tile.HasWall(direction))
                    {
                        UnityEngine.Debug.Log("Wall in current direction!");
                        newDirection = direction.RotateClockwise();
                        if (tile.HasWall(newDirection))
                        {
                            UnityEngine.Debug.Log("Wall in new direction!");
                            newDirection = direction.RotateCounterClockwise();
                            if (tile.HasWall(newDirection))
                            {
                                newDirection = direction.Flip();
                                UnityEngine.Debug.Log("Wall in new direction AGAIN!");
                            }
                        }
                    }

                    switch (newDirection)
                    {
                        case eDirection.North:
                            pos.z += remainingDistance;
                            break;

                        case eDirection.South:
                            pos.z -= remainingDistance;
                            break;

                        case eDirection.West:
                            pos.x -= remainingDistance;
                            break;

                        case eDirection.East:
                            pos.x += remainingDistance;
                            break;
                    }
                    pos.z += remainingDistance;

                    chunkTranslations[i] = new Translation
                    {
                        Value = pos
                    };
                    chunkDirections[i] = new Direction
                    {
                        Value = newDirection
                    };
                }
                else
                {
                    chunkTranslations[i] = new Translation
                    {
                        Value = translation + DeltaTime * speed * direction.GetWorldDirection()
                    };
                }
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

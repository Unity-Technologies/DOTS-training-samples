using System.Threading;
using DOTSRATS;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

static class NativeArrayExt
{
    public unsafe static ref T GetRef<T>(this NativeArray<T> array, int index)
        where T : unmanaged
            => ref ((T*) array.GetUnsafePtr())[index];
}

[UpdateAfter(typeof(SetupSystem))]
public class Movement : SystemBase
{
    EntityCommandBufferSystem CommandBufferSystem;

    struct ScoreUpdate
    {
        public int Cats;
        public int Rats;
    }

    protected override void OnCreate()
    {
        CommandBufferSystem
            = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameState>();
        RequireSingletonForUpdate<CellStruct>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        const float k_maxAnimalSpeed = 5;
        if (deltaTime > 0.5f / k_maxAnimalSpeed)
        {
            deltaTime = 0.5f / k_maxAnimalSpeed;
            Debug.LogWarning($"Game running too slow: Time.DeltaTime capped to {deltaTime}");
        }

        var gameStateEntity = GetSingletonEntity<GameState>();
        var gameState = EntityManager.GetComponentData<GameState>(gameStateEntity);

        var cellStructs = GetBuffer<CellStruct>(gameStateEntity);

        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var cdfe = GetComponentDataFromEntity<Goal>();

        int numPlayers = GetEntityQuery(ComponentType.ReadOnly<Player>()).CalculateEntityCount();

        var scoreUpdates = new NativeArray<ScoreUpdate>(numPlayers, Allocator.TempJob);

        Dependency = Entities
            .WithAll<InPlay>()
            .WithNativeDisableContainerSafetyRestriction(cdfe)
            .WithReadOnly(cellStructs)
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Velocity velocity) =>
            {
                var newTranslation = translation.Value + deltaTime * velocity.Direction.ToFloat3() * velocity.Speed;

                // Are we crossing the center of a tile?
                var tileCenter = (int3)(newTranslation + 0.5f);
                if (newTranslation.x >= tileCenter.x != translation.Value.x >= tileCenter.x ||
                    newTranslation.z >= tileCenter.z != translation.Value.z >= tileCenter.z)
                {
                    var cell = cellStructs[tileCenter.z * gameState.boardSize + tileCenter.x];

                    if (cell.hole)
                    {
                        velocity.Direction = Direction.Down;
                        newTranslation = new float3(tileCenter.x, translation.Value.y - math.length(newTranslation.xz - tileCenter.xz), tileCenter.z);
                        ecb.SetComponent(entityInQueryIndex, entity, new Scaling { targetScale = 0.1f });
                        ecb.AddComponent(entityInQueryIndex, entity, new Death { deathTimer = 5f });
                    }
                    else if (cell.goal != default)
                    {
                        var goal = cdfe[cell.goal];
                        if (HasComponent<Rat>(entity))
                            Interlocked.Increment(ref scoreUpdates.GetRef(goal.playerNumber).Rats);
                        else if (HasComponent<Cat>(entity))
                            Interlocked.Increment(ref scoreUpdates.GetRef(goal.playerNumber).Cats);
                        // TODO: Give this a pretty animation...
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                        ecb.SetComponent(cell.goal.Index, cell.goal, new Scale { Value = 1.5f });
                    }
                    else if (cell.wallLayout == (Direction.North | Direction.South | Direction.East | Direction.West))
                    {
                        // This will only happen if something exists within a completely walled system. The spawners
                        // should probably identify this situation and immediately set velocity/direction to None.
                        velocity.Direction = Direction.None;
                        velocity.Speed = 0f;
                        translation.Value = new float3(tileCenter.x, translation.Value.y, tileCenter.z);
                    }
                    else if (cell.arrow != Direction.None || cell.wallLayout != Direction.None)
                    {
                        if (cell.arrow != Direction.None)
                            velocity.Direction = cell.arrow;
                        if ((velocity.Direction & cell.wallLayout) != Direction.None)
                        {
                            var proposedDirection = RotateClockWise(velocity.Direction);
                            if ((proposedDirection & cell.wallLayout) != Direction.None)
                            {
                                proposedDirection = RotateCounterClockWise(velocity.Direction);
                                if ((proposedDirection & cell.wallLayout) != Direction.None)
                                    proposedDirection = RotateCounterClockWise(proposedDirection);
                            }
                            velocity.Direction = proposedDirection;
                        }

                        // Tweak new translation according to the (possible) new direction
                        newTranslation =
                            new float3(tileCenter.x, newTranslation.y, tileCenter.z) +
                            math.length(newTranslation.xz - tileCenter.xz) * velocity.Direction.ToFloat3();
                    }
                }

                translation.Value = newTranslation;
            }).ScheduleParallel(Dependency);

        Dependency = Entities
            .WithDisposeOnCompletion(scoreUpdates)
            .WithReadOnly(scoreUpdates)
            .ForEach((ref Player player) =>
            {
                var update = scoreUpdates[player.playerNumber];
                player.score += update.Rats;
                player.score = (int)(player.score * math.pow(0.666666f, update.Cats));
            }).ScheduleParallel(Dependency);
    }

    static Direction RotateClockWise(Direction dir)
    {
        // Optimization: use bitshifting
        switch (dir)
        {
            case Direction.North: return Direction.East;
            case Direction.South: return Direction.West;
            case Direction.East:  return Direction.South;
            case Direction.West:  return Direction.North;
            case Direction.Up:    return Direction.Up;
            case Direction.Down:  return Direction.Down;
            case Direction.None:
            default:              return Direction.None;
        }
    }
    
    static Direction RotateCounterClockWise(Direction dir)
    {
        // Optimization: use bitshifting
        switch (dir)
        {
            case Direction.North: return Direction.West;
            case Direction.South: return Direction.East;
            case Direction.East:  return Direction.North;
            case Direction.West:  return Direction.South;
            case Direction.Up:    return Direction.Up;
            case Direction.Down:  return Direction.Down;
            case Direction.None:
            default:              return Direction.None;
        }
    }
}

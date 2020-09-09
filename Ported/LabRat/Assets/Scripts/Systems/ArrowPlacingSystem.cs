using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class ArrowPlacingSystem : SystemBase
{
    EntityQuery tilesQuery;
    EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        tilesQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<PositionXZ>(),
                ComponentType.ReadOnly<Tile>(),
            }
        });

        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var tilePositions =
            tilesQuery.ToComponentDataArrayAsync<PositionXZ>(Allocator.TempJob, out var tilePositionsHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, tilePositionsHandle);

        var tiles =
            tilesQuery.ToComponentDataArrayAsync<Tile>(Allocator.TempJob, out var tilesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, tilesHandle);

        var tileEntities =
            tilesQuery.ToEntityArrayAsync(Allocator.TempJob, out var tileEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, tileEntitiesHandle);

        var ecb = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        var tileAccessor = GetComponentDataFromEntity<Tile>();

        Entities
            .WithDisposeOnCompletion(tilePositions)
            .WithDisposeOnCompletion(tiles)
            .WithDisposeOnCompletion(tileEntities)
            .WithNativeDisableParallelForRestriction(tileAccessor)
            .ForEach((
                int entityInQueryIndex,
                in Entity placeArrowEventEntity,
                in PositionXZ position,
                in PlaceArrowEvent placeArrowEvent,
                in Direction direction) =>
            {
                var arrowPosition = (int2)position.Value;
                for (int i = 0; i < tileEntities.Length; i++)
                {


                    var tilePosition = (int2)tilePositions[i].Value;
                    if (math.all(tilePosition != arrowPosition))
                        continue;

                    tileAccessor[tileEntities[i]] = new Tile
                    {
                        Value = (Tile.Attributes)(((byte)tiles[i].Value & ~(byte)Tile.Attributes.ArrowAny) | direction.Value << 4)
                    };


                }

                ecb.DestroyEntity(entityInQueryIndex, placeArrowEventEntity);
            }).ScheduleParallel();
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

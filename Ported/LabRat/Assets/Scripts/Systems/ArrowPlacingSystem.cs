using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
//using Color = UnityEngine.Color;

public class ArrowPlacingSystem : SystemBase
{
    Entity gameStateEntity;

    EntityQuery arrowsQuery;
    EntityQuery arrowPrefabQuery;
    EntityCommandBufferSystem ECBSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        arrowsQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Arrow>(),
                ComponentType.ReadOnly<Translation>(),
            }
        });

        arrowPrefabQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Arrow>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Prefab>()
            }
        });

        ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        gameStateEntity = EntityManager.CreateEntity(typeof(WantsGameStateTransitions));
        m_TileEntityGrid = new NativeArray<Entity>(0, Allocator.Persistent);
    }

    // Stolen from AnimalMovementSystem begin:
    // Collect tile entities into a flat array. This should probably come from the board generator in some shape or
    // form, but for now we'll just grab it at startup.
    void EnsureTileEntityGrid()
    {
        if (!EntityManager.HasComponent<GameStateInitialize>(gameStateEntity))
            return;

        // Dispose previous grid
        m_TileEntityGrid.Dispose();

        // Capture board dimensions
        var boardAuthoring = GetSingleton<BoardCreationAuthor>();
        m_BoardDimensions.TileCountX = boardAuthoring.SizeX;
        m_BoardDimensions.TileCountTotal = boardAuthoring.SizeX * boardAuthoring.SizeY;

        // Build a direct lookup structure for tile entities
        var tileEntityGrid = m_TileEntityGrid = new NativeArray<Entity>(m_BoardDimensions.TileCountTotal, Allocator.Persistent);
        var boardDimensions = m_BoardDimensions;
        Entities.WithName("CollectTiles")
            .WithAll<Tile>()
            .WithChangeFilter<Tile>()
            .ForEach((Entity entity, in PositionXZ pos) => { tileEntityGrid[AnimalMovementSystem.TileKeyFromPosition(pos.Value, boardDimensions)] = entity; })
            .ScheduleParallel();
    }

    AnimalMovementSystem.BoardDimensions m_BoardDimensions;
    NativeArray<Entity> m_TileEntityGrid;

    protected override void OnDestroy()
    {
        m_TileEntityGrid.Dispose();
    }
    //Stolen from AnimalMovementSystem end:

    protected override void OnUpdate()
    {
        EnsureTileEntityGrid();
        var boardDimensions = m_BoardDimensions;
        var tileEntityGrid = m_TileEntityGrid;

        var arrowPositions = arrowsQuery.ToComponentDataArrayAsync<Translation>(Allocator.TempJob, out var arrowPositionHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, arrowPositionHandle);
        var arrows = arrowsQuery.ToComponentDataArrayAsync<Arrow>(Allocator.TempJob, out var arrowsHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, arrowsHandle);
        var arrowEntities = arrowsQuery.ToEntityArrayAsync(Allocator.TempJob, out var arrowEntitiesHandle);
        Dependency = JobHandle.CombineDependencies(Dependency, arrowEntitiesHandle);
        //var arrowAccessor = GetComponentDataFromEntity<Arrow>();

        var ecb = ECBSystem.CreateCommandBuffer().AsParallelWriter();

        var arrowPrefab = arrowPrefabQuery.GetSingletonEntity();

        Entities
            .WithDisposeOnCompletion(arrowPositions)
            .WithDisposeOnCompletion(arrows)
            .WithDisposeOnCompletion(arrowEntities)
            .WithChangeFilter<PlaceArrowEvent>()
            .ForEach((
                int entityInQueryIndex,
                in Entity placeArrowEventEntity,
                in PositionXZ position,
                in PlaceArrowEvent placeArrowEvent,
                in Direction direction) =>
            {
                var tileEntity = tileEntityGrid[AnimalMovementSystem.TileKeyFromPosition(position.Value, boardDimensions)];
                var tile = GetComponent<Tile>(tileEntity);

                if ((tile.Value & Tile.Attributes.ArrowAny) != 0) // Target tile is an Arrow
                {
                    for (int j = 0; j < arrowEntities.Length; j++)
                    {
                        var arrowPosition = (int2)arrowPositions[j].Value.xz;
                        if (math.any(position.Value != arrowPosition))
                            continue;

                        if (arrows[j].Owner == placeArrowEvent.Player) // Placer owns the arrow, remove
                        {
                            var arrowEntity = arrowEntities[j];

                            ecb.DestroyEntity(entityInQueryIndex, arrowEntity);
                            ecb.SetComponent(entityInQueryIndex, tileEntity, new Tile { Value = (Tile.Attributes)((int)tile.Value & ~(int)Tile.Attributes.ArrowAny) });

                            var arrowsBuffer = GetBuffer<PlayerArrow>(placeArrowEvent.Player);
                            for (int k = 0; k < arrowsBuffer.Length; k++)
                            {
                                if (arrowsBuffer[k].Arrow == arrowEntity)
                                {
                                    arrowsBuffer.RemoveAt(k);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if ((tile.Value & Tile.Attributes.ObstacleAny) != 0) // Tile is either a Hole or a Goal
                {

                }
                else // Tile is free to place New Arrow
                {
                    var arrowsBuffer = GetBuffer<PlayerArrow>(placeArrowEvent.Player);
                    if (arrowsBuffer.Length >= PlayerArrow.MaxArrowsPerPlayer)
                    {
                        var arrowToDestroyEntity = arrowsBuffer[0].Arrow;
                        var arrowToDestroy = GetComponent<Arrow>(arrowToDestroyEntity);

                        ecb.SetComponent(entityInQueryIndex, tileEntity, new Tile { Value = (Tile.Attributes)((int)tile.Value & ~(int)Tile.Attributes.ArrowAny) });
                        ecb.DestroyEntity(entityInQueryIndex, arrowToDestroyEntity);
                        arrowsBuffer.RemoveAt(0);
                    }


                    ecb.SetComponent(entityInQueryIndex, tileEntity, new Tile { Value = (Tile.Attributes)(((int)tile.Value & ~(int)Tile.Attributes.ArrowAny) | (int)direction.Value << (int)Tile.Attributes.ArrowShiftCount) });
                    var newArrow = ecb.Instantiate(entityInQueryIndex, arrowPrefab);

                    var playerColor = GetComponent<Color>(placeArrowEvent.Player);
                    ecb.SetComponent(entityInQueryIndex, newArrow, new Arrow { Owner = placeArrowEvent.Player, Tile = tileEntity });
                    ecb.SetComponent(entityInQueryIndex, newArrow, new Translation { Value = new float3(position.Value.x, 0, position.Value.y) });
                    ecb.SetComponent(entityInQueryIndex, newArrow, new Rotation { Value = quaternion.Euler(0, AnimalMovementSystem.RadiansFromDirection(direction.Value), 0) });
                    ecb.AddComponent(entityInQueryIndex, newArrow, new FreshArrowTag());
                    ecb.AddComponent(entityInQueryIndex, newArrow, new Color() { Value = playerColor.Value });
                }

                ecb.DestroyEntity(entityInQueryIndex, placeArrowEventEntity);
            }).Schedule();
        ECBSystem.AddJobHandleForProducer(Dependency);
        
        // We need to run this with "Fresh Arrows" as they now have valid Entity index and can be added to a buffer
        ecb = ECBSystem.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithAll<FreshArrowTag>()
            .ForEach((
            int entityInQueryIndex,
            in Entity arrowEntity,
            in Arrow arrow) =>
        {
            var arrowsBuffer = GetBuffer<PlayerArrow>(arrow.Owner);
            arrowsBuffer.Add(new PlayerArrow { Arrow = arrowEntity });
            ecb.RemoveComponent<FreshArrowTag>(entityInQueryIndex, arrowEntity);
        }).Schedule();
        ECBSystem.AddJobHandleForProducer(Dependency);
    }
}

using System.Globalization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

//[UpdateAfter(typeof(MapSpawningSystem))]
//[UpdateAfter(typeof(PlayerInputSystem))]
public partial class ArrowPlacerSystem : SystemBase
{

    private EndSimulationEntityCommandBufferSystem mECBSystem;

    protected override void OnCreate()
    {
        mECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        RequireSingletonForUpdate<GameRunning>();
        RequireSingletonForUpdate<MapData>();
        RequireSingletonForUpdate<Config>();
    }

    protected override void OnUpdate()
    {
        var ecb = mECBSystem.CreateCommandBuffer().AsParallelWriter();

        var config = GetSingleton<Config>();
        float2 cellSize = new float2(config.MapWidth, config.MapHeight);

        DynamicBuffer<TileData> mapTiles = GetBuffer<TileData>(GetSingletonEntity<MapData>());
        float time = (float)Time.ElapsedTime;


        var player = GetSingletonEntity<PlayerInputTag>();
        var playerPos = GetComponent<CursorPosition>(player);
        var ecbHoverArrow = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .WithAll<Player>()
            .WithNone<Score>()
            .WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled)
            .WithoutBurst()
            .ForEach((Entity entity, ref CursorPosition position) =>
        {
            position.Value = playerPos.Value;
            var coordinate = CalculateProjected(position.Value, cellSize, out var direction);
            if (CanPlaceArrow(config, mapTiles, coordinate))
            {
                if (HasComponent<Disabled>(entity))
                    ecbHoverArrow.RemoveComponent<Disabled>(entity);
                SetComponent(entity, new Translation { Value = new float3(coordinate.x, .1f, coordinate.y) });
                SetComponent(entity, new Rotation { Value = Rotate(direction) });
            }
            else
            {
                if (!HasComponent<Disabled>(entity))
                    ecbHoverArrow.AddComponent<Disabled>(entity);
            }
        }).Run();
        ecbHoverArrow.Playback(EntityManager);
        ecbHoverArrow.Dispose();

        Entities
            .WithAll<Player>()
            .ForEach((Entity playerEntity, int nativeThreadIndex, ref PlayerSpawnArrow shouldSpawn, ref ArrowsDeployed arrowsDeployed, in CursorPosition playerPos, in Color playerColor) =>
            {
                if (!shouldSpawn.Value)
                {
                    return;
                }

                var coordinate = CalculateProjected(playerPos.Value, cellSize, out var direction);
                if (CanPlaceArrow(config, mapTiles, coordinate))
                {
                    var arrow = ecb.Instantiate(nativeThreadIndex, config.ArrowPrefab);
                    ecb.SetComponent(nativeThreadIndex, arrow, new Arrow { PlacedTime = time });
                    ecb.SetComponent(nativeThreadIndex, arrow, new Tile { Coords = coordinate });
                    ecb.SetComponent(nativeThreadIndex, arrow, new Direction { Value = direction });
                    ecb.SetComponent(nativeThreadIndex, arrow, new URPMaterialPropertyBaseColor { Value = playerColor.Value });
                    ecb.SetComponent(nativeThreadIndex, arrow, new PlayerOwned { Owner = playerEntity });
                    ecb.SetComponent(nativeThreadIndex, arrow, new Translation
                    {
                        Value = new float3(coordinate.x, .1f, coordinate.y)
                    });
                    ecb.SetComponent(nativeThreadIndex, arrow, new Rotation
                    {
                        Value = Rotate(direction)
                    });
                }

                shouldSpawn.Value = false;
            }).Run();
        mECBSystem.AddJobHandleForProducer(Dependency);
    }


    private static int2 CalculateProjected(float2 coord, float2 cellSize, out DirectionEnum cellDirection)
    {
        cellDirection = DirectionEnum.North;

        var intCoord = new int2((int)math.round(coord.x), (int)math.round(coord.y));
        var tilePt = coord - intCoord;
        if (math.abs(tilePt.y) > math.abs(tilePt.x))
            cellDirection = tilePt.y < 0 ? DirectionEnum.North : DirectionEnum.South;
        else
            cellDirection = tilePt.x > 0 ? DirectionEnum.West : DirectionEnum.East;

        return intCoord;

    }

    private static bool CanPlaceArrow(in Config config, in DynamicBuffer<TileData> mapTiles, int2 coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < config.MapWidth &&
               coordinate.y >= 0 && coordinate.y < config.MapHeight &&
               !MapData.HasHole(config, mapTiles, coordinate);
    }
    private static quaternion Rotate(DirectionEnum direction)
    {
        var rot = direction switch
        {
            DirectionEnum.North => math.PI,
            DirectionEnum.East => math.PI * -0.5f,
            DirectionEnum.West => math.PI * 0.5f,
            _ => 0f
        };
        return quaternion.Euler(math.PI * 0.5f, rot, 0);
    }
}

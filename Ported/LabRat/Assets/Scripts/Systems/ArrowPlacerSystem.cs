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
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var config = GetSingleton<Config>();
        var maxArrowUsages = GetSingleton<MaxArrowUsagesPerPlayer>();

        float2 cellSize = new float2(config.MapWidth, config.MapHeight);

        var players = GetEntityQuery(ComponentType.ReadOnly<Score>()).ToEntityArray(Allocator.Temp);

        var arrowsQuery = GetEntityQuery(
            ComponentType.ReadOnly<PlayTime>(),
            ComponentType.ReadOnly<Tile>(),
            ComponentType.ReadOnly<Direction>());
        var arrows = arrowsQuery.ToEntityArray(Allocator.TempJob);

        (int, float, int)[] arrowsCount = new (int, float, int)[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            arrowsCount[i] = (0, float.MaxValue, -1);
        }

        if (arrows.Length > 0)
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                var playerEntity = GetComponent<Player>(arrows[i]);
                for (int j = 0; j < players.Length; j++)
                {
                    if (playerEntity.PlayerEntity == players[j])
                    {
                        var (count, time, loc) = arrowsCount[j];
                        var arrowTime = GetComponent<PlayTime>(arrows[i]);
                        arrowsCount[j] = time > arrowTime.Value ? (count + 1, arrowTime.Value, i) : (count + 1, time, loc);
                    }
                }
            }
        }


        for (int i = 0; i < players.Length; i++)
        {
            var shouldSpawn = GetComponent<PlayerSpawnArrow>(players[i]);
            if (shouldSpawn.Value)
            {
                if (arrowsCount[i].Item1 >= maxArrowUsages.MaxArrowUsages)
                {
                    ecb.DestroyEntity(arrows[arrowsCount[i].Item3]);
                }

                var playerPos = GetComponent<Position>(players[i]);
                var playerColor = GetComponent<Color>(players[i]);
                DirectionEnum direction = DirectionEnum.None;
                var coordinate = RaycastCellDirection(playerPos.Value, cellSize, out direction);
                if (direction != DirectionEnum.None &&
                    coordinate.x > 0 && coordinate.x < cellSize.x &&
                    coordinate.y > 0 && coordinate.y < cellSize.y)
                {
                    var arrow = ecb.Instantiate(config.ArrowPrefab);
                    ecb.SetComponent(arrow, new Tile(coordinate));
                    ecb.SetComponent(arrow, new Direction(direction));
                    ecb.SetComponent(arrow, new URPMaterialPropertyBaseColor { Value = playerColor.Value });
                    ecb.SetComponent(arrow, new PlayTime { Value = UnityEngine.Time.realtimeSinceStartup });
                    ecb.SetComponent(arrow, new Player { PlayerEntity = players[i] });
                    ecb.SetComponent(arrow, new Translation
                    {
                        Value = new float3(coordinate.x, .1f, coordinate.y)
                    });
                    float rot = 0f;
                    switch(direction)
                    {
                        case DirectionEnum.South:
                            rot = 180;
                            break;
                        case DirectionEnum.East:
                            rot = 90;
                            break;
                        case DirectionEnum.West:
                            rot = -90;
                            break;
                    }
                    ecb.SetComponent(arrow, new Rotation
                    {
                        Value = Quaternion.Euler(90, rot, 0)
                    });
                }
                SetComponent(players[i], new PlayerSpawnArrow { Value = false });
            }
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    public int2 RaycastCellDirection(Vector2 screenPos, float2 cellSize, out DirectionEnum cellDirection)
    {
        cellDirection = DirectionEnum.North;

        var ray = Camera.main.ScreenPointToRay(screenPos);

        float enter;

        var plane = new Plane(Vector3.up, new Vector3(0, cellSize.y * 0.5f, 0));

        if (!plane.Raycast(ray, out enter))
            return new int2(-1, -1);

        var worldPos = ray.GetPoint(enter);
        var coord = new float2(worldPos.x-3, worldPos.z-3);
        var intCoord = new int2((int)coord.x, (int)coord.y);
        var tilePt = coord - intCoord- 0.5f;
        if (Mathf.Abs(tilePt.y) > Mathf.Abs(tilePt.x))
            cellDirection = tilePt.y > 0 ? DirectionEnum.North : DirectionEnum.South;
        else
            cellDirection = tilePt.x > 0 ? DirectionEnum.East : DirectionEnum.West;

        return intCoord;

    }
}

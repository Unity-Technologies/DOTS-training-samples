using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public partial class ArrowPlacerSystem : SystemBase
{
    (int2, DirectionEnum) FromSceneToTableCoords(float2 position)
    {
        return (new int2((int)position.x, (int)position.y), DirectionEnum.North);
    }
    float2 cellSize;
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var config = GetSingleton<Config>();
        var mapSpawner = GetSingleton<MapSpawner>();
        var maxArrowUsages = GetSingleton<MaxArrowUsagesPerPlayer>();

        cellSize = new float2(mapSpawner.MapWidth, mapSpawner.MapHeight);

        var players = GetEntityQuery(ComponentType.ReadOnly<Score>()).ToEntityArray(Allocator.Temp);

        var arrowsQuery = GetEntityQuery(
            ComponentType.ReadOnly<PlayTime>(),
            ComponentType.ReadOnly<Tile>(),
            ComponentType.ReadOnly<Direction>());
        var arrows = arrowsQuery.ToEntityArray(Allocator.TempJob);

        (int, float, int)[] arrowsCount = new (int, float, int)[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            arrowsCount[i] = (0, 0f, -1);
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
                        arrowsCount[j] = time < arrowTime.Value ? (count + 1, arrowTime.Value, i) : (count + 1, time, loc);
                    }
                }
            }
        }


        for (int i = 0; i < players.Length; i++)
        {
            var shouldSpawn = GetComponent<PlayerSpawnArrow>(players[i]);
            if (shouldSpawn.Value)
            {
                if (arrowsCount[i].Item1 > maxArrowUsages.MaxArrowUsages)
                {
                    ecb.DestroyEntity(arrows[arrowsCount[i].Item3]);
                }

                var playerPos = GetComponent<Position>(players[i]);
                var playerColor = GetComponent<Color>(players[i]);

                var (coordinate, direction) = FromSceneToTableCoords(playerPos.Value);

                var arrow = ecb.Instantiate(config.ArrowPrefab);
                SetComponent(arrow, new Tile(coordinate));
                SetComponent(arrow, new Direction(direction));
                SetComponent(arrow, new URPMaterialPropertyBaseColor { Value = playerColor.Value });
                SetComponent(arrow, new PlayTime { Value = UnityEngine.Time.realtimeSinceStartup });
                SetComponent(arrow, new Player { PlayerEntity = players[i] });

                shouldSpawn.Value = false;
                SetComponent(players[i], shouldSpawn);
            }
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }


    public float2 CoordAtWorldPosition(Vector3 worldPosition)
    {
        //var localPt3D = boardTransform.InverseTransformPoint(worldPosition);
        var localPt = new float2(0, 0);
        localPt += cellSize * 0.5f; // offset by half cellsize
        return localPt / cellSize;
    }

    public int2 RaycastCellDirection(Vector2 screenPos, out DirectionEnum cellDirection)
    {
        cellDirection = DirectionEnum.North;

        var ray = Camera.main.ScreenPointToRay(screenPos);

        float enter;

        var plane = new Plane(Vector3.up, new Vector3(0, cellSize.y * 0.5f, 0));

        if (!plane.Raycast(ray, out enter))
            return new int2(-1, -1);

        var worldPos = ray.GetPoint(enter);
        var coord = CoordAtWorldPosition(worldPos);
        var intCoord = new int2((int)coord.x, (int)coord.y);
        var tilePt = coord - intCoord;
        if (Mathf.Abs(tilePt.y) > Mathf.Abs(tilePt.x))
            cellDirection = tilePt.y > 0 ? DirectionEnum.North : DirectionEnum.South;
        else
            cellDirection = tilePt.x > 0 ? DirectionEnum.East : DirectionEnum.West;

        return intCoord;

    }
}

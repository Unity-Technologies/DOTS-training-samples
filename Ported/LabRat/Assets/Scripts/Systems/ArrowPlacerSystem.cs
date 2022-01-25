using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
        //TODO: need cellsize;
        var mapSpawner = GetSingleton<MapSpawner>();
        cellSize = new float2(mapSpawner.MapWidth, mapSpawner.MapHeight);

        var maxArrowUsages = GetSingleton<MaxArrowUsagesPerPlayer>();
        var playerInputEntity = GetSingletonEntity<PlayerInputTag>();
        var playerPos = GetComponent<Position>(playerInputEntity);
        var playerColor = GetComponent<Color>(playerInputEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        if (Input.GetMouseButtonDown(0))
        {
            var (coordinate, direction) = FromSceneToTableCoords(playerPos.Value);

            var entity = ecb.CreateEntity();
            ecb.AddComponent<Tile>(entity, new Tile(coordinate));
            ecb.AddComponent<Direction>(entity, new Direction(direction));
            ecb.AddComponent<Color>(entity, playerColor);
            ecb.AddComponent(entity, new ArrowMiceCount(maxArrowUsages.MaxArrowUsages));
            //TODO: Add time since placed if we need it.
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

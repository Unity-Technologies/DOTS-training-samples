using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class PlayerInputSystem : SystemBase
{
    Plane plane;
    Camera camera;
    int2 lastPos;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BoardSize>();
        RequireSingletonForUpdate<CellData>();
        plane = new Plane(Vector3.up, new Vector3(0, 0.5f, 0));
    }

    protected override void OnUpdate()
    {
        if (camera == null)
            camera = Camera.main;

        var ray = camera.ScreenPointToRay(Input.mousePosition);

        float enter;

        if (!plane.Raycast(ray, out enter))
            return;

        var worldPos = ray.GetPoint(enter);

        int2 pos = new int2(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));

        var boardSize = GetSingleton<BoardSize>();

        var cellDataEntity = GetSingletonEntity<CellData>();
        var cellData = EntityManager.GetComponentObject<CellData>(cellDataEntity);

        if (pos.x >= 0 && pos.x < boardSize.Value.x && pos.y >= 0 && pos.y < boardSize.Value.y)
        {
            if (pos.x != lastPos.x || pos.y != lastPos.y)
            {
                var lastCellEntity = cellData.cells[lastPos.y * boardSize.Value.x + lastPos.x];

                if (!EntityManager.HasComponent<ArrowTag>(lastCellEntity))
                {
                    var lastCellLinks = EntityManager.GetComponentData<CellComponentLink>(lastCellEntity);
                    EntityManager.SetComponentData(lastCellLinks.arrow, new Color { Value = float4.zero });
                    EntityManager.SetComponentData(lastCellLinks.arrowOutline, new Color { Value = float4.zero });
                }
            }

            var arrayPos = pos.y * boardSize.Value.x + pos.x;
            var cellEntity = cellData.cells[arrayPos];

            if (!EntityManager.HasComponent<DisableRendering>(cellEntity))
            {
                var cellLinks = EntityManager.GetComponentData<CellComponentLink>(cellEntity);

                if (EntityManager.HasComponent<ArrowTag>(cellEntity))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        cellData.directions[arrayPos] = 0;
                        EntityManager.RemoveComponent<ArrowTag>(cellEntity);
                        EntityManager.SetComponentData(cellLinks.arrow, new Color { Value = float4.zero });
                        EntityManager.SetComponentData(cellLinks.arrowOutline, new Color { Value = float4.zero });
                    }
                }
                else
                {
                    float2 cellOffset = new float2(worldPos.x - pos.x, worldPos.z - pos.y);

                    DirectionEnum arrowDirection = math.abs(cellOffset.y) > math.abs(cellOffset.x)
                        ? (cellOffset.y > 0 ? DirectionEnum.North : DirectionEnum.South)
                        : (cellOffset.x > 0 ? DirectionEnum.East : DirectionEnum.West);

                    if (Input.GetMouseButtonDown(0))
                    {
                        cellData.directions[arrayPos] = (byte)(1 << (int)arrowDirection);
                        EntityManager.AddComponent<ArrowTag>(cellEntity);
                        EntityManager.SetComponentData(cellEntity, new Direction { Value = arrowDirection });
                        EntityManager.SetComponentData(cellLinks.arrow, new Color { Value = new float4(1, 1, 1, 1) });
                        EntityManager.SetComponentData(cellLinks.arrowOutline, new Color { Value = new float4(0, 0, 0, 1) });
                    }
                    else
                    {
                        EntityManager.SetComponentData(cellLinks.arrow, new Color { Value = new float4(0.5f, 0.5f, 0.5f, 1) });
                        EntityManager.SetComponentData(cellLinks.arrowOutline, new Color { Value = new float4(0, 0, 0, 0) });
                    }

                    Quaternion rot;

                    switch (arrowDirection)
                    {
                        case DirectionEnum.North:
                            rot = quaternion.EulerXYZ(math.PI / 2f, 0, 0);
                            break;
                        case DirectionEnum.South:
                            rot = quaternion.EulerXYZ(math.PI / 2f, math.PI, 0);
                            break;
                        case DirectionEnum.East:
                            rot = quaternion.EulerXYZ(math.PI / 2f, math.PI / 2f, 0);
                            break;
                        case DirectionEnum.West:
                            rot = quaternion.EulerXYZ(math.PI / 2f, -math.PI / 2f, 0);
                            break;
                        default:
                            rot = quaternion.EulerXYZ(math.PI / 2f, 0, 0);
                            break;
                    }

                    EntityManager.SetComponentData(cellLinks.arrow, new Rotation { Value = rot });
                }
            }

            lastPos = pos;
        }
        else if (lastPos.x >= 0 && lastPos.x < boardSize.Value.x && lastPos.y >= 0 && lastPos.y < boardSize.Value.y)
        {
            var lastCellEntity = cellData.cells[lastPos.y * boardSize.Value.x + lastPos.x];

            if (!EntityManager.HasComponent<ArrowTag>(lastCellEntity))
            {
                var lastCellLinks = EntityManager.GetComponentData<CellComponentLink>(lastCellEntity);
                EntityManager.SetComponentData(lastCellLinks.arrow, new Color { Value = float4.zero });
                EntityManager.SetComponentData(lastCellLinks.arrowOutline, new Color { Value = float4.zero });
            }
        }
    }
}

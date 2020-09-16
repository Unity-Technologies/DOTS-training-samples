using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : SystemBase
{
    Plane plane;
    Camera camera;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<CellData>();
        plane = new Plane(Vector3.up, new Vector3(0, 0.5f, 0));
    }

    protected override void OnUpdate()
    {
        if (camera == null)
            camera = Camera.main;

        if (Input.GetMouseButtonDown(0))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            float enter;

            if (!plane.Raycast(ray, out enter))
			    return;

            var worldPos = ray.GetPoint(enter);

            int2 pos = new int2(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));

            float2 cellOffset = new float2(worldPos.x - pos.x, worldPos.z - pos.y);

            DirectionEnum arrowDirection = math.abs(cellOffset.y) > math.abs(cellOffset.x) 
                ? (cellOffset.y > 0 ? DirectionEnum.North : DirectionEnum.South) 
                : (cellOffset.x > 0 ? DirectionEnum.East : DirectionEnum.West);

            Debug.Log(arrowDirection);

            var cellDataEntity = GetSingletonEntity<CellData>();
            var cellData = EntityManager.GetComponentObject<CellData>(cellDataEntity);

            var cellEntity = cellData.cells[pos.y * 13 + pos.x];

            EntityManager.SetComponentData(cellEntity, new Color { Value = new float4(1, 0, 0, 1) });
        }
    }
}

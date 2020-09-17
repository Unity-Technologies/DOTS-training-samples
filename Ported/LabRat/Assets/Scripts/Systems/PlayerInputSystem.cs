using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PlayerInputSystem : SystemBase
{
    Plane plane;
    Camera camera;

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

        if (Input.GetMouseButtonDown(0))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            float enter;

            if (!plane.Raycast(ray, out enter))
			    return;

            var worldPos = ray.GetPoint(enter);

            int2 pos = new int2(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));

            var boardSize = GetSingleton<BoardSize>();

            if (pos.x > 0 && pos.x < boardSize.Value.x && pos.y > 0 && pos.y < boardSize.Value.y)
            {
                var cellDataEntity = GetSingletonEntity<CellData>();
                var cellData = EntityManager.GetComponentObject<CellData>(cellDataEntity);

                var cellEntity = cellData.cells[pos.y * boardSize.Value.x + pos.x];

                if (EntityManager.HasComponent<ArrowTag>(cellEntity))
                {
                    EntityManager.RemoveComponent<ArrowTag>(cellEntity);
                }
                else
                {
                    float2 cellOffset = new float2(worldPos.x - pos.x, worldPos.z - pos.y);

                    DirectionEnum arrowDirection = math.abs(cellOffset.y) > math.abs(cellOffset.x)
                        ? (cellOffset.y > 0 ? DirectionEnum.North : DirectionEnum.South)
                        : (cellOffset.x > 0 ? DirectionEnum.East : DirectionEnum.West);

                    EntityManager.AddComponent<ArrowTag>(cellEntity);
                    EntityManager.SetComponentData(cellEntity, new Direction { Value = arrowDirection });
                }
            }
        }
    }
}

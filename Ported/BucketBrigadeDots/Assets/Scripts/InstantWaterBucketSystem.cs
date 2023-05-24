
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct InstantWaterBucketSystem : ISystem
{
    private const float FireSize = .3f;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
        state.RequireForUpdate<FireTemperature>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!Input.GetMouseButtonUp(0)) return;

        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, new Vector3(0f, -.5f, 0f));
        if (!plane.Raycast(mouseRay, out var enter)) return;

        var worldPoint = mouseRay.origin + mouseRay.direction * enter;
        var waterPos = new int2((int)math.round(worldPoint.x / FireSize), (int)math.round(worldPoint.z / FireSize));

        WaterCellAndNeighbors(waterPos);
    }

    private void WaterCellAndNeighbors(int2 centerPos)
    {
        var settings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();

        var cols = settings.RowsAndColumns;
        var size = settings.Size;

        for (var yD = -1; yD < 2; yD++)
        {
            for (var xD = -1; xD < 2; xD++)
            {
                var x = centerPos.x + xD;
                var y = centerPos.y + yD;
                if (x >= 0 && x < cols && y >= 0 && y < cols)
                {
                    var index = y * cols + x;
                    temperatures[index] = 0f;
                }
            }
        }
    }
}
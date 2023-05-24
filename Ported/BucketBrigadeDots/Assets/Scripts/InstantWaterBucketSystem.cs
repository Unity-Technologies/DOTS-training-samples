
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
        var plane = new Plane(Vector3.up, Vector3.zero);
        if (!plane.Raycast(mouseRay, out var enter)) return;

        var worldPoint = mouseRay.origin + mouseRay.direction * enter;
        var waterPos = new float2(worldPoint.x / FireSize, worldPoint.z / FireSize);

        var settings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();

        var cols = settings.RowsAndColumns;
        var size = settings.Size;
        
        for (var i = 0; i < size; i++)
        {
            var firePos = new float2(i % cols, i / cols);
            var distanceSq = math.distancesq(waterPos, firePos);
            var waterEfficiency = math.max(0f, 5f - distanceSq);
            if (waterEfficiency > 0f)
            {
                temperatures[i] = math.max(0f, temperatures[i] - waterEfficiency);
            }
        }
    }
}
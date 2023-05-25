using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct InstantWaterBucketSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
        state.RequireForUpdate<FireTemperature>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        if (!Input.GetMouseButtonUp(0)) return;

        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.up, new Vector3(0f, -.5f, 0f));
        if (!plane.Raycast(mouseRay, out var enter)) return;

        var worldPoint = ((float3)(mouseRay.origin + mouseRay.direction * enter)).xz;
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();
        SystemUtilities.PutoutFire(in worldPoint, in gameSettings, ref temperatures);
    }
}
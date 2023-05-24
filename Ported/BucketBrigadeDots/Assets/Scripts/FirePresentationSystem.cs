using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct FirePresentationSystem : ISystem
{
    private static readonly float4 green = new float4(0f, 1f, 0f, 1f);
    private static readonly float4 red = new float4(1f, 0f, 0f, 1f);
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FireCell>();
        state.RequireForUpdate<GameSettings>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameSettings = SystemAPI.GetSingleton<GameSettings>();
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>();

        var index = 0;
        var time = (float)SystemAPI.Time.ElapsedTime * 10f;
        foreach (var (localTransform, baseColor, entity) in SystemAPI.Query<RefRW<LocalTransform>,RefRW<URPMaterialPropertyBaseColor>>().WithAll<FireCell>().WithEntityAccess())
        {
            var heat = temperatures[index];
            var x = index % gameSettings.RowsAndColumns;
            var z = index / gameSettings.RowsAndColumns;
            var sine = math.sin(time + entity.Index) + 1f;
            var y = heat - heat * sine * .05f - 1f;
            localTransform.ValueRW.Position = new float3(x * .3f, y, z * .3f);

            baseColor.ValueRW.Value = math.lerp(green, red, heat);
            
            index++;
        }
    }
}

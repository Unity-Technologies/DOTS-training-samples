using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[UpdateAfter(typeof(LineSpawner))]
[BurstCompile]
public partial struct StationSpawner : ISystem
{

    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var line in SystemAPI.Query<RefRO<Line>>())
        {
            var stations = new NativeArray<Entity>(4, Allocator.Temp);
            ecb.Instantiate(config.StationPrefab, stations);

            var lineColor = line.ValueRO.LineColor;
            var color = new URPMaterialPropertyBaseColor { Value = lineColor };
        }
}
}

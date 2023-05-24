using System;
using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct PheromoneManagementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ant>();
        state.RequireForUpdate<Config>();
    }

    public void OnUpdate(ref SystemState state)
    {
        if (PheromoneTextureView.Instance == null)
            return;
        
        var config = SystemAPI.GetSingleton<Config>();
        if (PheromoneTextureView.PheromoneTex == null)
            PheromoneTextureView.Initialize(config.MapSize);

        var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>();
        PheromoneTextureView.PheromoneTex.SetPixelData(pheromones.AsNativeArray().Reinterpret<byte>(), 0);
        PheromoneTextureView.PheromoneTex.Apply();
    }
}

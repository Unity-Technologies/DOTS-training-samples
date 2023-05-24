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
        var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>();

        if (PheromoneTextureView.PheromoneTex == null)
        {
            PheromoneTextureView.Initialize(config.MapSize);
            
            //Pheromone buffer setup
            pheromones.Length = config.MapSize * config.MapSize;
            for (int i = 0; i < pheromones.Length; i++)
            {
                pheromones[i] = new Pheromone(){Value = 0};
            }
        }

        var pixels = pheromones.AsNativeArray().Reinterpret<short>();
        
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = (short)(pixels[i] * config.PheromoneDecay);
        }
        
        PheromoneTextureView.PheromoneTex.SetPixelData(pixels, 0);
        PheromoneTextureView.PheromoneTex.Apply();
    }
}

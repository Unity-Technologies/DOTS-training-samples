using System;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial struct PheromoneManagementSystem : ISystem
{
    private bool HasInitedBuffer;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        HasInitedBuffer = false;
        state.RequireForUpdate<Ant>();
        state.RequireForUpdate<Config>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var pheromones = SystemAPI.GetSingletonBuffer<Pheromone>();//Pheromone buffer setup
        if (!HasInitedBuffer)
        {
            pheromones.Length = config.MapSize * config.MapSize;
            for (int i = 0; i < pheromones.Length; i++)
            {
                pheromones[i] = new Pheromone() { Value = 0 };
            }

            HasInitedBuffer = true;
        }
        
        if (PheromoneTextureView.Instance == null)
            return;
        

        if (PheromoneTextureView.PheromoneTex == null)
        {
            PheromoneTextureView.Initialize(config.MapSize);
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

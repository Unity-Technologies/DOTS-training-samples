using Unity.Entities;
using UnityEngine;

public partial struct PheromonesInitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var entity = ecb.CreateEntity();
        
        #if UNITY_EDITOR
        ecb.SetName(entity, $"THIS IS A BUFFER");
        #endif
        
        var buffer = ecb.AddBuffer<PheromoneBufferElement>(entity);
        
        var totalPixels = globalSettings.MapSizeX * globalSettings.MapSizeY;
        buffer.Capacity = totalPixels;
        
        for (int i = 0; i < totalPixels; i++)
        {
            buffer.Add(0);
        }
    }
}

public struct PheromoneBufferElement : IBufferElementData
{
    // These implicit conversions are optional, but can help reduce typing.
    public static implicit operator float(PheromoneBufferElement e) { return e.Value; }
    public static implicit operator PheromoneBufferElement(float e) { return new PheromoneBufferElement { Value = e }; }

    // Actual value each buffer element will store.
    public float Value;  // TODO: maybe int is better in terms of performance?
}


using System;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
///     Singleton data denoting important entity params.
/// </summary>
public struct AntSimulationRuntimeData : IComponentData
{
    public uint perFrameRandomSeed;
    
    public float2 colonyPos, foodPosition;
    public bool hasSpawnedAnts;
}

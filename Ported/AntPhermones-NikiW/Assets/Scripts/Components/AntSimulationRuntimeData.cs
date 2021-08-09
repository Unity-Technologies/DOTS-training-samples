using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

/// <summary>
///     Singleton data denoting important entity params.
/// </summary>
[GenerateAuthoringComponent]
public struct AntSimulationRuntimeData : IComponentData
{
    [GhostField]
    public float2 colonyPos, foodPosition;
    
    [GhostField]
    public bool hasSpawnedAnts;
}

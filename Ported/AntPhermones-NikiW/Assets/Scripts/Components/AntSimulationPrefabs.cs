using System;
using Unity.Burst;
using Unity.Entities;

/// <summary>
///     Singleton prefab data for the AntSimulation.
/// </summary>
[NoAlias]
[GenerateAuthoringComponent]
public struct AntSimulationPrefabs : IComponentData
{
    public Entity npcAntPrefab;
    public Entity playerAntPrefab;
    public Entity antSimulationRuntimeDataPrefab;
    public Entity foodPheromonesPrefab;
    public Entity colonyPheromonesPrefab;
}

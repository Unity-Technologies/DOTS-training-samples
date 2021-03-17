using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Constant parameters for the simulation
/// </summary>
[GenerateAuthoringComponent, Serializable]
public struct SimulationParameters : IComponentData
{
    public float CanScrollSpeed;
    public float RockScrollSpeed;
}

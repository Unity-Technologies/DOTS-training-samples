using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct SimulationSpeed : IComponentData
{
    public float Value;
}
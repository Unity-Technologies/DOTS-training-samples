using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WaterAmount : IComponentData
{
    public float Value;
}

public struct WaterRefill : IComponentData { }

using Unity.Entities;
using Unity.Mathematics;

// Fire simulation grid settings
[GenerateAuthoringComponent]
public struct FireGridSettings : IComponentData
{
    public uint2 FireGridResolution;
}

public struct FireCell : IBufferElementData
{
    public float FireTemperature;
}
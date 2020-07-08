using Unity.Entities;
using Unity.Mathematics;

// Fire simulation grid settings
[GenerateAuthoringComponent]
public struct FireGridSettings : IComponentData
{
    public int2 FireGridResolution;
}

public struct FireCell : IBufferElementData
{
    public float FireTemperature;
}







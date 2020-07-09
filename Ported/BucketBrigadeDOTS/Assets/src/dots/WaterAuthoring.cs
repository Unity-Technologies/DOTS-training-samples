using Unity.Entities;
using Unity.Mathematics;

// Fire simulation grid settings
[GenerateAuthoringComponent]
public struct WaterVolume : IComponentData
{
    public float Volume;
}
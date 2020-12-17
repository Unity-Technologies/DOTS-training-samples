using Unity.Entities;

[GenerateAuthoringComponent]
public struct Crop : IComponentData
{
    public float GrowthRate;
    public float FullGrowthValue;
    public float CurrentGrowth;
}
using Unity.Entities;

[GenerateAuthoringComponent]
public struct MaxArrowUsagesPerPlayer : IComponentData
{
    public int MaxArrowUsages;
}
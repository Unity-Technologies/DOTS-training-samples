using Unity.Entities;

[GenerateAuthoringComponent]
public struct DroppingOffBucket : IComponentData
{
    public bool DroppingOff;
    public SharedChainComponent chain;
}
using Unity.Entities;

[GenerateAuthoringComponent]
public struct Agent : IComponentData
{
    public float ChainT;
    public Entity MyChain;
}

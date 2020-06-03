using Unity.Entities;

[GenerateAuthoringComponent]
public struct Agent : IComponentData
{
    public float ChainT;
    public float Forward;
    public Entity MyChain;
}

using Unity.Entities;

[GenerateAuthoringComponent]
public struct NextInChain : IComponentData
{
    public Entity Next;
}
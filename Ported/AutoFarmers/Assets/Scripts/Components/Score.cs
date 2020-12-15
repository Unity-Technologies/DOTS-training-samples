using Unity.Entities;

[GenerateAuthoringComponent]
public struct Score : IComponentData
{
    public int Value;
}

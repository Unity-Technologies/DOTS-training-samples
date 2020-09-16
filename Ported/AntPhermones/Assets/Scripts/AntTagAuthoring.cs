using Unity.Entities;

[GenerateAuthoringComponent]
public struct AntTag : IComponentData
{
    public const float Size = 1.5f;
    public bool HasFood;
}
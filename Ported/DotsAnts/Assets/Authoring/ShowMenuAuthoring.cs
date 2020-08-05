using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ShowMenu : IComponentData
{
    public bool value;
}
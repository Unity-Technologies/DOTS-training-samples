using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ScreenSize : IComponentData
{
    public int Value;
}
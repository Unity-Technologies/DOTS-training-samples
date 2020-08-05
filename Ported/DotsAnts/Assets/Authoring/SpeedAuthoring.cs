using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Speed : IComponentData
{
    public float value;
}
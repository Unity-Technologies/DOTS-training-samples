using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct LineId : IComponentData
{
    public int Value;
}
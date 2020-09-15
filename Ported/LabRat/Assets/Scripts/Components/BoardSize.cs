using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BoardSize : IComponentData
{
    public int2 Value;
}

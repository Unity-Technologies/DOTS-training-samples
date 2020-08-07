using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Offset : IComponentData
{
    public half Value;

    public static implicit operator float(Offset d)
    {
        return d.Value;
    }
}

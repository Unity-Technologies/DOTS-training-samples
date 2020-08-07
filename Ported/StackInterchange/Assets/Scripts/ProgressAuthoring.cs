using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Progress : IComponentData
{
    public half Value;

    public static implicit operator float(Progress d)
    {
        return d.Value;
    }
}

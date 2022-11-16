using Unity.Entities;
using Unity.Mathematics;

public struct Resource : IComponentData
{
    public bool Dead;
    public bool IsTopOfStack;
}
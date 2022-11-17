using Unity.Entities;
using Unity.Mathematics;

public struct Resource : IComponentData
{
    public bool Dead;
    public Entity Holder;
    public int2 GridIndex;
}
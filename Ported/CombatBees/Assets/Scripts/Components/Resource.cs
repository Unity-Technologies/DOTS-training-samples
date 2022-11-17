using Unity.Entities;
using Unity.Mathematics;

public struct Resource : IComponentData
{
    public bool Dead;
    public Entity Holder;
    public int2 GridIndex;
    /// <summary>
    /// The resource directly underneath if it is in a stack
    /// </summary>
    public Entity ResourceUnder;
}
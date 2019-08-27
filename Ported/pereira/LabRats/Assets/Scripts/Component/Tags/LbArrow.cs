using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Tag to identify that is needed to add a new arrow
/// </summary>
public struct LbArrow : IComponentData
{
    public int2 Location;
    public byte Direction;
}
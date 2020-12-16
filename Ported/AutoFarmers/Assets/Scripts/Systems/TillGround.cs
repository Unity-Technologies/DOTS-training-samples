using Unity.Entities;
using Unity.Mathematics;

public struct TillGround : IComponentData 
{
}
public struct TillArea : IComponentData
{
    public int2 Position;
    public int2 Size;
}

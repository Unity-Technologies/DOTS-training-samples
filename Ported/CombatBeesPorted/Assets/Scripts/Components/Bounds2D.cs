using Unity.Entities;
using Unity.Mathematics;

public struct Bounds2D: IComponentData
{
    public float2 Center => Position + Size / 2;
    
    public float2 Position;
    public float2 Size;
}
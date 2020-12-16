using Unity.Entities;
using Unity.Mathematics;

public struct Rock : IComponentData
{
    public float Health;
    public int2 Size;
    public int2 Position;
}
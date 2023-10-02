using Unity.Entities;
using Unity.Mathematics;

public struct Ant : IComponentData
{
    //public float2 Position;
    public float Speed;
    public float Angle;
    public bool HasFood;
}
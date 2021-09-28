using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct AntMovement : IComponentData
{
    public float2 Direction;
    public float2 Position;
    public byte State;
}
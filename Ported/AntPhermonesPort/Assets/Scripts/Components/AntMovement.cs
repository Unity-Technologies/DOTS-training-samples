using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public struct AntMovement : IComponentData
{
    public float2 Direction;
    public float2 Position;
    public byte State;
    public float MoveSpeed;
}
using Unity.Entities;
using Unity.Mathematics;

public struct MoveInfo : IComponentData
{
    public float2 destinationPosition;
    public float speed;
}

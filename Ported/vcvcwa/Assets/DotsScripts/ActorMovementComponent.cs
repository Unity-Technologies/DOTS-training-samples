using Unity.Entities;
using Unity.Mathematics;

public struct ActorMovementComponent : IComponentData
{
    public float2 targetPosition;
    public float2 position;
    public float speed;
}


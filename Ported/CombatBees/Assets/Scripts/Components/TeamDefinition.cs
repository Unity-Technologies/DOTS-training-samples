using Unity.Entities;
using Unity.Mathematics;

public struct TeamDefinition : IBufferElementData
{
    public float speed;
    public float aggression;
    public float attackRange;
    public float pickupFoodRange;
    public float huntTimeout;
    public float3 flutterMagnitude;
    public float3 flutterInterval;
    public Entity hive;
}


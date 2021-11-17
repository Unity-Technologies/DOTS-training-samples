using Unity.Entities;

public struct TeamDefinition : IBufferElementData
{
    public float speed;
    public float aggression;
    public float attackRange;
    public float pickupFoodRange;
    public float huntTimeout;
    public Entity hive;
}


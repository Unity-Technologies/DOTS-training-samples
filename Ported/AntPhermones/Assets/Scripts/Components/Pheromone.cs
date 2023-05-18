using Unity.Entities;

public struct LookingForFoodPheromone : IBufferElementData
{
    public float strength;
}

public struct LookingForHomePheromone : IBufferElementData
{
    public float strength;
}

using Unity.Entities;

// An empty component is called a "tag component".
public struct FoodResource : IComponentData
{
    public FoodState State; // by default FoodState.NULL
}
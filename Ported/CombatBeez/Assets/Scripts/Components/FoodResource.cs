using Unity.Entities;

// An empty component is called a "tag component".
struct FoodResource : IComponentData
{
    public FoodState State = FoodState.NULL;
}
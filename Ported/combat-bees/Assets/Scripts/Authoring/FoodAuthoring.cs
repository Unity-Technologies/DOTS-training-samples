using Unity.Entities;

class FoodAuthoring : UnityEngine.MonoBehaviour
{
}

class FoodBaker : Baker<FoodAuthoring>
{
    public override void Bake(FoodAuthoring authoring)
    {
        AddComponent<Velocity>();
        AddComponent<Dead>();
    }
}
using Unity.Entities;
using UnityEngine;

public class FoodAuthoring : MonoBehaviour
{
    
}

class FoodBaker : Baker<FoodAuthoring>
{
    public override void Bake(FoodAuthoring authoring)
    {
        AddComponent<Food>();
    }
}
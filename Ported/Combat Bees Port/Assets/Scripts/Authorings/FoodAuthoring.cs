using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class FoodAuthoring : MonoBehaviour
{
    
}

class FoodBaker : Baker<FoodAuthoring>
{
    public override void Bake(FoodAuthoring authoring)
    {
        AddComponent(new Food
        {
            targetPos = float3.zero
        });
    }
}
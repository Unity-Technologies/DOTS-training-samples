using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

class FoodAuthoring : UnityEngine.MonoBehaviour
{
}

class FoodBaker : Baker<FoodAuthoring>
{
    public override void Bake(FoodAuthoring authoring)
    {
        AddComponent<Velocity>();
        AddComponent<Faction>();
    }
}
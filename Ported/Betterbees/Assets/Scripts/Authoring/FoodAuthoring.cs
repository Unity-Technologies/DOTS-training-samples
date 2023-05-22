using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FoodAuthoring : MonoBehaviour
{
}

public class FoodBaker : Baker<FoodAuthoring>
{
    public override void Bake(FoodAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new GravityComponent());
        AddComponent(entity, new FoodComponent());
    }
}
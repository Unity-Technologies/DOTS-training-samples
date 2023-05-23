using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class FoodAuthoring : MonoBehaviour
{
}

public class FoodBaker : Baker<FoodAuthoring>
{
    public override void Bake(FoodAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new LocalTransform
        {
            Position = authoring.transform.position,
            Rotation = authoring.transform.rotation,
            Scale = authoring.transform.localScale.magnitude
        });
        
        AddComponent(entity, new GravityComponent());
        
        AddComponent(entity, new FoodComponent());
    }
}
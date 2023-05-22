using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BloodAuthoring : MonoBehaviour
{
}

public class BloodBaker : Baker<BloodAuthoring>
{
    public override void Bake(BloodAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new GravityComponent());
        AddComponent(entity, new BloodComponent());
    }
}
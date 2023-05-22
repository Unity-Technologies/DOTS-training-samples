using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FireCellAuthoring : MonoBehaviour
{
    
}

public class FireCellBaker : Baker<FireCellAuthoring>
{
    public override void Bake(FireCellAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.WorldSpace);

        AddComponent(entity, new FireCell {
            FlameValue = 0
        });
    }
}

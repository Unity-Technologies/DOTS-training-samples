using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WaterCellAuthoring : MonoBehaviour
{
    public float MaxWaterValue = 1f;
}

public class WaterCellBaker : Baker<WaterCellAuthoring>
{
    public override void Bake(WaterCellAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new WaterCell {
            MaxWaterValue = authoring.MaxWaterValue,
            WaterValue = authoring.MaxWaterValue
        });
    }
}
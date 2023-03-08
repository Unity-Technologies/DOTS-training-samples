using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;


public struct TargetPosition : IComponentData
{
    public float3 value;
}
public class TargetPositionAuthoring : MonoBehaviour
{
    public float3 value;
}

public class TargetPositionBaker : Baker<TargetPositionAuthoring>
{
    public override void Bake(TargetPositionAuthoring authoring)
    {
        AddComponent(new TargetPosition{value = authoring.value});
    }
}
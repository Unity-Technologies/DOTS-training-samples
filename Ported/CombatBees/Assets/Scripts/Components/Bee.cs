using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Bee : IComponentData
{
    public float3 TargetOffset;
    public Entity TargetEntity;
    public Entity CarriedFoodEntity;
    public float TimeLeftTilIdleUpdate;
}

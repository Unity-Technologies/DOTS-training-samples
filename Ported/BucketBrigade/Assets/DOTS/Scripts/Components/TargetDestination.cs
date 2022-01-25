using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TargetDestination : IComponentData
{
    public static implicit operator float2(TargetDestination e) { return e.Value; }
    public static implicit operator TargetDestination(float2 e) { return new TargetDestination { Value = e }; }

    public float2 Value;
}

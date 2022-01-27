using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[Serializable]
public struct TargetDestination : IComponentData
{
    public static implicit operator float2(TargetDestination e) { return e.Value; }
    public static implicit operator TargetDestination(float2 e) { return new TargetDestination { Value = e }; }

    public float2 Value;

    public float DistanceToDestinationSq(Translation t) => math.lengthsq(t.Value.xz - Value);
    public bool IsAtDestination(Translation t) => DistanceToDestinationSq(t) < 0.0001f;
}

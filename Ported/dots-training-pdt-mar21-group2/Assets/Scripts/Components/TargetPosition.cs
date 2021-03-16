using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
// Used for knowing position of a target ie Can
public struct TargetPosition : IComponentData
{
    public float3 Value;
}

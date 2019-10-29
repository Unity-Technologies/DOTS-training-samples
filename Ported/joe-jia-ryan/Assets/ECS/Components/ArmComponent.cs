using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct ArmComponent : IComponentData
{
    public float3 armPosition;
    public float3 handPosition;
    
    //TODO IK bone weights, finger sizes, etc.
}

using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent, Serializable]
public struct AngularVelocity : IComponentData
{
    public float3 Value;    
}

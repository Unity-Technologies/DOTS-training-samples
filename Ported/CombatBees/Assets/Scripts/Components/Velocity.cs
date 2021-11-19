using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


[GenerateAuthoringComponent] 
[Serializable]
public struct Velocity : IComponentData
{
    public float3 Value;
}

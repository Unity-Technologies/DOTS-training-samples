using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TargetEntity : IComponentData
{
    public Entity Value;
    //public float3 ValuePosition;
}
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TargetVelocity : IComponentData
{
    public float Value;
}
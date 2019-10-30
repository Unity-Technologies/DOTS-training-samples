using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct ChaseVelocity : IComponentData
{
    public float Value;
}
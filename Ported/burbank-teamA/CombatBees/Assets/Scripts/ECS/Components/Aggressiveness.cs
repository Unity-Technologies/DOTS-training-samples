using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]

public struct Aggressiveness : IComponentData
{
    public float Value;
}
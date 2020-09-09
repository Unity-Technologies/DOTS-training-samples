using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
[WriteGroup(typeof(LocalToWorld))]
public struct Falling : IComponentData
{
    public float Value;
}


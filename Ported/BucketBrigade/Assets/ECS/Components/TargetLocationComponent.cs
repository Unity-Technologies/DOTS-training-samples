using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct TargetLocationComponent : IComponentData
{
    public float2 location;
}

using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct FireFighter : IComponentData
{
}

public struct HoldsEmptyBucket : IComponentData { }
public struct HoldsFullBucket : IComponentData { }
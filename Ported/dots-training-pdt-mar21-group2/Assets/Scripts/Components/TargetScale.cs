using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TargetScale : IComponentData
{
    public float Value;
}
